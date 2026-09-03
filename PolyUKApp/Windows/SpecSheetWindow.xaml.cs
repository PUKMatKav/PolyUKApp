using PolyUKApp.Core;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace PolyUKApp.Windows
{
    /// <summary>
    /// Row shown in the results ListBox — wraps a ProductSpec with a
    /// display-friendly label.
    /// </summary>

    public class ProductSpecListItem
    {
        public ProductSpec Spec { get; }
        public string DisplayLabel => $"{Spec.Code} — {Spec.Name}";


        public ProductSpecListItem(ProductSpec spec)
        {
            Spec = spec;
        }
    }

    /// <summary>
    /// Interaction logic for SpecSheetWindow.xaml
    /// </summary>
    /// 
    public partial class SpecSheetWindow : Window
    {
        private static bool _questPdfLicenseSet = false;
        private readonly Dictionary<string, System.Windows.Controls.TextBox> _fieldEditors = new();
        private string? GetFieldValue(string label, string? fallback) =>
        _fieldEditors.TryGetValue(label, out var tb) ? tb.Text : fallback;

        public SpecSheetWindow()
        {
            InitializeComponent();

            // QuestPDF Community license — free under their revenue threshold,
            // see questpdf.com/license. Only needs setting once per process.
            if (!_questPdfLicenseSet)
            {
                QuestPDF.Settings.License = LicenseType.Community;
                _questPdfLicenseSet = true;
            }
        }

        private void TopBar0_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void BtnMinimise_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnMaximise_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string term = SearchTextBox.Text.Trim();
            if (term.Length < 2)
            {
                ResultsListBox.ItemsSource = null;
                StatusText.Text = "Type at least 2 characters to search.";
                return;
            }

            StatusText.Text = "Searching...";

            try
            {
                var results = await System.Threading.Tasks.Task.Run(() => SageSpecRepository.SearchByCodeOrName(term));

                ResultsListBox.ItemsSource = results.Select(r => new ProductSpecListItem(r)).ToList();
                StatusText.Text = results.Count == 0
                    ? "No matching items found."
                    : $"{results.Count} item(s) found" + (results.Count == 100 ? " (showing first 100)" : "") + ".";
            }
            catch (Exception ex)
            {
                StatusText.Text = "Search failed — check Sage connection.";
                System.Windows.MessageBox.Show($"Could not search Sage:\n{ex.Message}", "Spec Sheet Generator",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResultsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResultsListBox.SelectedItem is not ProductSpecListItem item)
            {
                BtnGeneratePdf.IsEnabled = false;
                return;
            }

            DisplaySpec(item.Spec);
            BtnGeneratePdf.IsEnabled = true;
        }

        private void DisplaySpec(ProductSpec spec)
        {
            SelectedItemHeader.Text = spec.Name;

            FieldsPanel.Children.Clear();
            _fieldEditors.Clear();

            FieldsPanel.Children.Clear();
            AddFieldRow("Item Code", spec.Code);
            AddFieldRow("Dimensions", spec.Dimensions);
            AddFieldRow("Material", spec.Material);
            AddFieldRow("Gauge / Thickness", spec.Gauge);
            AddFieldRow("Unit", spec.Unit);
            AddFieldRow("Colour", spec.Colour);
            AddFieldRow("Slip Level", spec.SlipLevel);
            AddFieldRow("Additives / Treatment", spec.AdditivesTreatment);
            AddFieldRow("Pallet Quantity", spec.PalletQuantity?.ToString());
            AddFieldRow("Case Quantity", spec.CaseQuantity?.ToString());
            AddFieldRow("Yield (per roll)", spec.Yield);
            AddFieldRow("Printed", spec.Printed);
            if (!string.IsNullOrWhiteSpace(spec.Perforations))
                AddFieldRow("Perforations", spec.Perforations);
            AddFieldRow("Weight", spec.Weight);
            AddFieldRow("Film Type", spec.FilmType);
            AddFieldRow("Format", spec.Format);
            AddFieldRow("QC Process", "QC checked by factory staff after production and monitored during manufacture");
            AddFieldRow("Tolerance", "+/- 10%");
            AddFieldRow("Batch Coding / Traceability", "Unique 6 digit batch number");

            LeftoverTextBlock.Text = string.IsNullOrWhiteSpace(spec.LeftoverText)
                ? "(none)"
                : spec.LeftoverText;

            RawDescriptionTextBlock.Text = spec.RawDescription;
        }

        private void AddFieldRow(string label, string? value)
        {

            var row = new StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };

            row.Children.Add(new TextBlock
            {
                Text = label,
                Width = 140,
                FontFamily = new System.Windows.Media.FontFamily("Aptos"),
                FontWeight = FontWeights.SemiBold,
                Foreground = (System.Windows.Media.Brush)FindResource("Text")
            });

            var textBox = new System.Windows.Controls.TextBox
            {
                Text = value ?? "",
                FontFamily = new System.Windows.Media.FontFamily("Aptos"),
                Foreground = (System.Windows.Media.Brush)FindResource("Text"),
                Background = System.Windows.Media.Brushes.Transparent,
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = (System.Windows.Media.Brush)FindResource("BorderMid"),
                Padding = new Thickness(2),
                MinWidth = 200
            };

            row.Children.Add(textBox);
            FieldsPanel.Children.Add(row);

            _fieldEditors[label] = textBox;
        }

        private void BtnGeneratePdf_Click(object sender, RoutedEventArgs e)
        {
            if (ResultsListBox.SelectedItem is not ProductSpecListItem item) return;

            // Use whatever the user has edited in the Additional Notes box,
            // not the original parsed text.
            item.Spec.LeftoverText = LeftoverTextBlock.Text;
            item.Spec.Weight = GetFieldValue("Weight", item.Spec.Weight);
            item.Spec.Code = GetFieldValue("Item Code", item.Spec.Code);
            item.Spec.Dimensions = GetFieldValue("Dimensions", item.Spec.Dimensions);
            item.Spec.Material = GetFieldValue("Material", item.Spec.Material);
            item.Spec.Gauge = GetFieldValue("Gauge / Thickness", item.Spec.Gauge);
            item.Spec.Colour = GetFieldValue("Colour", item.Spec.Colour);
            item.Spec.Yield = GetFieldValue("Yield (per roll)", item.Spec.Yield);
            item.Spec.FilmType = GetFieldValue("Film Type", item.Spec.FilmType);
            item.Spec.Format = GetFieldValue("Format", item.Spec.Format);
            item.Spec.Unit = GetFieldValue("Unit", item.Spec.Unit);
            item.Spec.Perforations = GetFieldValue("Perforations", item.Spec.Perforations);
            item.Spec.Printed = GetFieldValue("Printed", item.Spec.Printed);
            item.Spec.SlipLevel = GetFieldValue("Slip Level", item.Spec.SlipLevel);
            item.Spec.AdditivesTreatment = GetFieldValue("Additives / Treatment", item.Spec.AdditivesTreatment);
            // Pallet Quantity / Case Quantity are int? on the model — parse or clear
            item.Spec.PalletQuantity = int.TryParse(GetFieldValue("Pallet Quantity", null), out int pq) ? pq : null;
            item.Spec.CaseQuantity = int.TryParse(GetFieldValue("Case Quantity", null), out int cq) ? cq : null;

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save Spec Sheet",
                Filter = "PDF file (*.pdf)|*.pdf",
                FileName = $"{SanitizeFileName(item.Spec.Code)}_SpecSheet.pdf"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                var document = new SpecSheetDocument(item.Spec);
                document.GeneratePdf(dialog.FileName);

                var result = System.Windows.MessageBox.Show("Spec sheet generated. Open it now?", "Spec Sheet Generator",
                    MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
                {
                    Process.Start(new ProcessStartInfo(dialog.FileName) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Could not generate PDF:\n{ex.Message}", "Spec Sheet Generator",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string SanitizeFileName(string name)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }
    }
}
