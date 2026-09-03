using System.Text.RegularExpressions;

namespace PolyUKApp.Core
{

    public static class DescriptionParser
    {
        private static string FormatPct(double value) =>
        value % 1 == 0 ? value.ToString("0") : value.ToString("0.##");

        // Shared number pattern — allows comma thousands separators, e.g. "1,320".
        private const string Number = @"\d+(?:,\d{3})*(?:\.\d+)?";

        private static readonly Regex GaugeRegex = new(
            @$"{Number}\s*(?:micron|mic|mu|gauge|ga|μm|g)\b",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex YieldRegex = new(
            @$"(?:approx\.?\s*)?{Number}\s*(?:mtrs?|m|metres?|meters?)\s*per\s*roll",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Perforations On Roll — "POR300" or "POR 300".
        private static readonly Regex PorRegex = new(
            @$"POR\s*(?<count>{Number})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Pre-stretch percentage — "250% PPS" or spelled out "250% Power
        // Pre-Stretch". Must run BEFORE Dimensions: otherwise "x 250%"
        // reads like a fake second dimension and Dimensions swallows it.
        private static readonly Regex PpsRegex = new(
            @$"{Number}\s*%\s*(?:PPS\b|Power\s+Pre-?Stretch\b)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private const string LengthUnit = @"(?:(?:mm|cm|mt|mtr|metre|meter|m|in)\b|"")";
        private const string DimConnector = @"\s*[xX×/]\s*";

        private static readonly Regex DimensionsRegex = new(
            $@"(?:[LWH]\s*)?{Number}\s*{LengthUnit}?{DimConnector}(?:[LWH]\s*)?{Number}\s*{LengthUnit}?({DimConnector}(?:[LWH]\s*)?{Number}\s*{LengthUnit}?)?",
            RegexOptions.Compiled);


        // Fallback for a single spatial dimension with no pair nearby —
        // e.g. "500mm x 23mu" is width x GAUGE, not two dimensions, so
        // once gauge is stripped only "500mm" is left on its own.
        private static readonly Regex SingleDimensionRegex = new(
            $@"{Number}\s*{LengthUnit}",
            RegexOptions.Compiled);

        // "Printed 2 Colours/2 Sides", "Printed 1 Colour"
        private static readonly Regex PrintedVerboseRegex = new(
            @"Printed\s+(?<colours>\d+)\s*Colou?rs?(?:\s*/\s*(?<sides>\d+)\s*Sides?)?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Shorthand "1c/1s", "2C/2S"
        private static readonly Regex PrintedShorthandRegex = new(
            @"\b(?<colours>\d+)\s*C\s*/\s*(?<sides>\d+)\s*S\b",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex RecycledContentRegex = new(
            @$"(?<pct>{Number})\s*%\s*(?<type>PCW|PIR|Recycled)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly string[] MaterialKeywords =
        {
            "LDPE", "HDPE", "LLDPE", "MDPE", "Polythene", "Polypropylene", "PP",
            "PVC", "PET", "Biodegradable", "Compostable"
        };

        private static readonly string[] SlipLevelKeywords =
        {
            "Low Slip", "Medium Slip", "High Slip", "Standard Slip"
        };

        // Fallback list only used if PpsRegex didn't already set FilmType.
        private static readonly string[] FilmTypeKeywords =
        {
            "HP Shrink", "Shrink", "Cast", "Blown", "Stretch"
        };

        private static readonly string[] FormatKeywords =
        {
            "CFS", "Lay-Flat", "Flat", "BW Bags", "Bags", "GLFT", "Machine Film", "Hand Wrap", "LFT", "Covers", "Cover", "Sleeves", "Sleeve", "Sheet", "Sheets", "Gusseted Bags", "Gusseted Covers"
        };

        private static readonly string[] ColourKeywords =
        {
            "Blue Tint", "Red Tint", "Green Tint", "Clear", "Natural", "Black", "White", "Blue", "Red", "Green",
            "Yellow", "Opaque", "Transparent", "Amber"
        };

        private static readonly Regex PackagingQtyRegex = new(
            @$"(?<qty>{Number})\s*[a-zA-Z]*\s*(?:/|per)\s*(?<container>pallets?|plts?|boxes?|cases?|ctns?|cartons?)\b",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex PalletQtyReverseRegex = new(
            @"pallet\s*(?:qty|quantity)?\s*[:\-]?\s*(\d{1,6})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex CaseQtyReverseRegex = new(
            @"(?:case|box|ctn|carton)\s*(?:qty|quantity)?\s*[:\-]?\s*(\d{1,6})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Percentage-based additives — extend this alternation as you find more
        // (e.g. "12% FR" and "12% Flame Retardant" both normalize to the same
        // output). Deliberately NOT a generic "N% anything" pattern — that would
        // also catch product names like "5% TUFF", which isn't an additive.
        private static readonly Regex AdditivePctRegex = new(
            @$"(?<pct>{Number})\s*%\s*(?<name>Anti-Static|Flame\s*Retardant|FR)\b",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Surface treatments — no percentage attached.
        private static readonly Regex CoronaTreatmentRegex = new(
            @"Corona\s*Treat(?:ment|ed)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static ProductSpec Parse(string code, string name, string rawDescription)
        {
            var spec = new ProductSpec
            {
                Code = code,
                Name = name,
                RawDescription = rawDescription
            };

            string working = rawDescription ?? "";

            spec.Gauge = ExtractAndStrip(ref working, GaugeRegex);
            spec.Printed = ExtractPrinted(ref working);
            if (string.IsNullOrWhiteSpace(spec.Printed))
                spec.Printed = "Unprinted";
            spec.Yield = ExtractAndStrip(ref working, YieldRegex);
            var porMatch = PorRegex.Match(working);
            if (porMatch.Success)
            {
                spec.Perforations = porMatch.Groups["count"].Value;
                working = working.Remove(porMatch.Index, porMatch.Length);
            }

            // Extract PPS before Dimensions — see comment on PpsRegex above.
            string? ppsValue = ExtractAndStrip(ref working, PpsRegex);

            spec.Dimensions = ExtractAndStrip(ref working, DimensionsRegex)
                            ?? ExtractAndStrip(ref working, SingleDimensionRegex);

            string? recycledPct = null;
            string? recycledType = null;
            var recycledMatch = RecycledContentRegex.Match(working);
            if (recycledMatch.Success)
            {
                recycledPct = recycledMatch.Groups["pct"].Value;
                recycledType = recycledMatch.Groups["type"].Value.ToUpperInvariant();
                working = working.Remove(recycledMatch.Index, recycledMatch.Length);
            }
            spec.Material = ExtractKeywordAndStrip(ref working, MaterialKeywords);

            // Fall back to LDPE when no material keyword was found — otherwise
            // items with recycled content but no explicit material name (e.g.
            // "30% PCW" with no "LDPE" nearby) would show nothing at all here.
            if (string.IsNullOrWhiteSpace(spec.Material))
                spec.Material = "LDPE";

            if (recycledPct != null && double.TryParse(recycledPct.Replace(",", ""), out double pct))
            {
                double virginPct = 100 - pct;
                spec.Material = $"{FormatPct(virginPct)}% Virgin {spec.Material} + {FormatPct(pct)}% {recycledType} {spec.Material}";
            }
            else
            {
                spec.Material = $"100% Virgin {spec.Material}";
            }

            ExtractPackagingQuantities(ref working, spec);
            spec.SlipLevel = ExtractKeywordAndStrip(ref working, SlipLevelKeywords);
            if (string.IsNullOrWhiteSpace(spec.SlipLevel))
                spec.SlipLevel = "Standard Slip";
            ExtractAdditives(ref working, spec);
            if (string.IsNullOrWhiteSpace(spec.AdditivesTreatment))
                spec.AdditivesTreatment = "None";
            //spec.Material = ExtractKeywordAndStrip(ref working, MaterialKeywords);
            spec.FilmType = ppsValue ?? ExtractKeywordAndStrip(ref working, FilmTypeKeywords);
            spec.Format = ExtractKeywordAndStrip(ref working, FormatKeywords);

            spec.Colour = ExtractKeywordAndStrip(ref working, ColourKeywords);
            if (string.IsNullOrWhiteSpace(spec.Colour))
                spec.Colour = "Natural";

            spec.LeftoverText = CleanupLeftover(working);

            return spec;
        }

        private static void ExtractPackagingQuantities(ref string text, ProductSpec spec)
        {
            var matches = PackagingQtyRegex.Matches(text)
                .Cast<Match>()
                .OrderByDescending(m => m.Index)
                .ToList();

            foreach (var m in matches)
            {
                string container = m.Groups["container"].Value.ToLowerInvariant();
                if (!int.TryParse(m.Groups["qty"].Value.Replace(",", ""), out int qty)) continue;

                if (container.StartsWith("pallet") || container.StartsWith("plt"))
                    spec.PalletQuantity = qty;
                else
                    spec.CaseQuantity = qty;

                text = text.Remove(m.Index, m.Length);
            }

            if (spec.PalletQuantity == null)
            {
                var m = PalletQtyReverseRegex.Match(text);
                if (m.Success && int.TryParse(m.Groups[1].Value, out int qty))
                {
                    spec.PalletQuantity = qty;
                    text = text.Remove(m.Index, m.Length);
                }
            }

            if (spec.CaseQuantity == null)
            {
                var m = CaseQtyReverseRegex.Match(text);
                if (m.Success && int.TryParse(m.Groups[1].Value, out int qty))
                {
                    spec.CaseQuantity = qty;
                    text = text.Remove(m.Index, m.Length);
                }
            }
        }

        private static string? ExtractAndStrip(ref string text, Regex regex)
        {
            var match = regex.Match(text);
            if (!match.Success) return null;

            text = text.Remove(match.Index, match.Length);
            return match.Value.Trim();
        }

        private static string? ExtractKeywordAndStrip(ref string text, string[] keywords)
        {
            foreach (var keyword in keywords)
            {
                var regex = new Regex($@"\b{Regex.Escape(keyword)}\b", RegexOptions.IgnoreCase);
                var match = regex.Match(text);
                if (match.Success)
                {
                    text = text.Remove(match.Index, match.Length);
                    return keyword;
                }
            }
            return null;
        }

        private static string CleanupLeftover(string text)
        {
            var cleaned = Regex.Replace(text, @"(?<=\s)[xX×](?=\s|$)", " ");
            cleaned = Regex.Replace(cleaned, @"[,\-/]{2,}", " ");
            cleaned = Regex.Replace(cleaned, @"[ \t]{2,}", " ");
            cleaned = Regex.Replace(cleaned, @"\n{2,}", "\n");
            cleaned = cleaned.Trim(' ', ',', '-', '/', '\n', '\t');
            return cleaned;
        }
        private static string? ExtractPrinted(ref string text)
        {
            var match = PrintedVerboseRegex.Match(text);
            if (!match.Success)
                match = PrintedShorthandRegex.Match(text);

            if (!match.Success) return null;

            text = text.Remove(match.Index, match.Length);

            string colours = match.Groups["colours"].Value;
            string sides = match.Groups["sides"].Success ? match.Groups["sides"].Value : "";

            return sides != "" ? $"{colours}C/{sides}S" : $"{colours}C";
        }
        private static void ExtractAdditives(ref string text, ProductSpec spec)
        {
            var parts = new List<string>();

            // Process back-to-front so removing matched text doesn't shift the
            // indices of matches still to come.
            var matches = AdditivePctRegex.Matches(text).Cast<Match>().OrderByDescending(m => m.Index).ToList();
            foreach (var m in matches)
            {
                string pct = m.Groups["pct"].Value;
                string rawName = m.Groups["name"].Value;
                string name = Regex.IsMatch(rawName, "flame|FR", RegexOptions.IgnoreCase)
                    ? "Flame Retardant"
                    : "Anti-Static";

                parts.Add($"{pct}% {name}");
                text = text.Remove(m.Index, m.Length);
            }
            parts.Reverse(); // restore original left-to-right order

            var coronaMatch = CoronaTreatmentRegex.Match(text);
            if (coronaMatch.Success)
            {
                parts.Add("Corona Treated");
                text = text.Remove(coronaMatch.Index, coronaMatch.Length);
            }

            if (parts.Count > 0)
                spec.AdditivesTreatment = string.Join(", ", parts);
        }
    }
}