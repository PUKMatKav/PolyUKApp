namespace PolyUKApp.Core
{
    /// <summary>
    /// A Sage stock item plus the fields parsed out of its unstructured
    /// Description text.
    /// </summary>
    public class ProductSpec
    {
        // --- Raw, from Sage StockItem ---
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string RawDescription { get; set; } = "";

        // --- Parsed fields (null = not found in the text) ---
        public string? Dimensions { get; set; }
        public string? Material { get; set; }
        public string? Gauge { get; set; }
        public string? Colour { get; set; }
        public int? PalletQuantity { get; set; }
        public int? CaseQuantity { get; set; }
        public string? Weight { get; set; }   // now populated from Sage's Weight column, not parsed
        public string? Yield { get; set; }              // e.g. "approx. 106mtrs per roll"
        public string? FilmType { get; set; }           // e.g. "Cast", "Blown", "HP Shrink"
        public string? Format { get; set; }             // e.g. "CFS", "Gusseted"
        public string? Unit { get; set; }   // from Sage's StockUnitName column, e.g. "Roll", "Box", "Each"
        public string? Perforations { get; set; }   // e.g. "300" from "POR 300" / "POR300"
        public string? Printed { get; set; }   // normalized e.g. "2C/2S", "1C" (no sides specified)
        public string? SlipLevel { get; set; }   // e.g. "Low Slip", "Medium Slip"
        public string? AdditivesTreatment { get; set; }   // e.g. "2% Anti-Static, Corona Treated"

        // Anything the regexes didn't recognise, so nothing gets silently lost.
        public string LeftoverText { get; set; } = "";
    }
}