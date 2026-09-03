using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Windows;

namespace PolyUKApp.Core
{
    /// <summary>
    /// Renders a single ProductSpec as a one-page PDF spec sheet.
    /// </summary>
    public class SpecSheetDocument : IDocument
    {
        private readonly ProductSpec _spec;
        private static byte[]? _logoBytes;

        public SpecSheetDocument(ProductSpec spec)
        {
            _spec = spec;
        }

        private static byte[] GetLogoBytes()
        {
            if (_logoBytes != null) return _logoBytes;

            var uri = new Uri("pack://application:,,,/Windows/Images/Polythene_UK_Logo_2022_RGB_LR.png");
            var streamInfo = System.Windows.Application.GetResourceStream(uri);

            using var ms = new MemoryStream();
            streamInfo!.Stream.CopyTo(ms);
            _logoBytes = ms.ToArray();
            return _logoBytes;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header().Column(col =>
                {
                    col.Item().PaddingTop(-20).AlignLeft().Height(70).Image(GetLogoBytes());
                    col.Item().PaddingTop(30).Text("Product Specification Sheet")
                        .FontSize(20).Bold().FontColor(Colors.Blue.Darken3);
                    col.Item().PaddingTop(2).Text(_spec.Name)
                        .FontSize(13).FontColor(Colors.Grey.Darken2);
                    col.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Blue.Darken2);
                });

                page.Content().PaddingTop(15).Column(col =>
                {
                    col.Spacing(5);

                    AddField(col, "Item Code", _spec.Code);
                    AddField(col, "Dimensions", _spec.Dimensions);
                    AddField(col, "Material", _spec.Material);
                    AddField(col, "Gauge / Thickness", _spec.Gauge);
                    AddField(col, "Unit", _spec.Unit);
                    AddField(col, "Colour", _spec.Colour);
                    AddField(col, "Slip Level", _spec.SlipLevel);
                    AddField(col, "Additives / Treatment", _spec.AdditivesTreatment);
                    AddField(col, "Pallet Quantity", _spec.PalletQuantity?.ToString());
                    AddField(col, "Case Quantity", _spec.CaseQuantity?.ToString());
                    AddField(col, "Yield (per roll)", _spec.Yield);
                    AddField(col, "Printed", _spec.Printed);
                    AddField(col, "POR", _spec.Perforations);
                    AddField(col, "Weight", _spec.Weight);
                    AddField(col, "Film Type", _spec.FilmType);
                    AddField(col, "Format", _spec.Format);
                    AddField(col, "QC Process", "QC checked by factory staff after production and monitored during manufacture");
                    AddField(col, "Tolerance", "+/- 10%");
                    AddField(col, "Batch Coding / Traceability", "Unique 6 digit batch number");

                    if (!string.IsNullOrWhiteSpace(_spec.LeftoverText))
                    {
                        col.Item().PaddingTop(10).Text("Additional Notes").Bold().FontColor(Colors.Blue.Darken2);
                        col.Item().Text(_spec.LeftoverText);
                    }

                    col.Item().PaddingTop(15).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                    col.Item().PaddingTop(8).Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken1));

                        bool isBrc = _spec.Code.StartsWith("BRC", StringComparison.OrdinalIgnoreCase);

                        text.Line(isBrc
                            ? "All LDPE polymer is ethically sourced as part of our ISO / BRCGS certifications and conforms to European and USA FDA food contact regulations. It can be expected that materials and articles made from this polymer product will pass the overall migration tests for all food types in normal applications."
                            : "All LDPE polymer is ethically sourced as part of our ISO / BRCGS certifications.");
                        text.Line("");
                        text.Line("The above product conforms to all required UK/EU standards not limited to but including PPWR EU 2025/40, UK General Product Safety Regulations, REACH Regulation (EC) No. 1907/2006.");
                        text.Line("");
                        text.Line("All our films are recyclable (except compostable films) and no additives are used which impede recyclability.");
                        text.Line("");
                        text.Line("Please note: We work to +/- 10% PAFA tolerances.");
                    });
                });
                page.Footer().Column(col =>
                {
                    col.Item().AlignLeft().Text("4 Witan Park, Avenue Two, Witney, OX28 4FH")
                        .FontSize(8).FontColor(Colors.Grey.Medium);
                    col.Item().AlignLeft().Text("01993 777950 / sales@polytheneuk.co.uk")
                        .FontSize(8).FontColor(Colors.Grey.Medium);
                    col.Item().AlignLeft().Text(x =>
                    {
                        x.Span("Generated ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span(DateTime.Now.ToString("dd/MM/yyyy")).FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            });
        }

        private static void AddField(ColumnDescriptor col, string label, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            col.Item().Row(row =>
            {
                row.ConstantItem(140).Text(label).Bold();
                row.RelativeItem().Text(value);
            });
        }
    }
}