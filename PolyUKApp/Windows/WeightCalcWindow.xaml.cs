using System;
using System.Collections.Generic;
using System.IO;
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

namespace PolyUKApp.Windows
{
    /// <summary>
    /// Interaction logic for WeightCalcWindow.xaml
    /// </summary>

    public static class TextBoxBehavior
    {
        public static readonly DependencyProperty NumericOnlyProperty =
            DependencyProperty.RegisterAttached("NumericOnly", typeof(bool), typeof(TextBoxBehavior),
                new PropertyMetadata(false, OnNumericOnlyChanged));

        public static bool GetNumericOnly(DependencyObject obj) => (bool)obj.GetValue(NumericOnlyProperty);
        public static void SetNumericOnly(DependencyObject obj, bool value) => obj.SetValue(NumericOnlyProperty, value);

        private static void OnNumericOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is System.Windows.Controls.TextBox textBox)
            {
                if ((bool)e.NewValue)
                {
                    textBox.PreviewTextInput += BlockNonNumeric;
                    textBox.PreviewKeyDown += BlockSpace;
                    System.Windows.DataObject.AddPastingHandler(textBox, OnPaste);
                }

                else
                {
                    textBox.PreviewTextInput -= BlockNonNumeric;
                    textBox.PreviewKeyDown += BlockSpace;
                    System.Windows.DataObject.RemovePastingHandler(textBox, OnPaste);
                }
            }
        }

        private static void BlockNonNumeric(object sender, TextCompositionEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = (System.Windows.Controls.TextBox)sender;

            if (e.Text == ".")
            {
                // Block if a decimal point already exists
                e.Handled = textBox.Text.Contains('.');
            }
            else
            {
                e.Handled = !e.Text.All(char.IsDigit);
            }
        }
        private static void BlockSpace(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private static void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            bool isValid = e.DataObject.GetDataPresent(typeof(string)) &&
                           IsValidDecimal((string)e.DataObject.GetData(typeof(string)));

            if (!isValid)
            {
                e.CancelCommand();
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    System.Windows.MessageBox.Show("Only numeric values and a single decimal point are allowed.",
                                    "Invalid Paste",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                }));
            }
        }

        private static bool IsValidDecimal(string text)
        {
            return decimal.TryParse(text, out _);
        }
    }

    public partial class WeightCalcWindow : Window
    {
        bool WeightMeasuring = true;
        bool MeterMeasuring = false;

        double relativeDensity = 0.923;

        int bagtube = 1;
        double itemLength;
        double itemWidth;
        double itemthickness;
        double bagboxQuantity;
        double unitQuantity;
        double unitWeightTotal;
        double totalWeightTotal;

        double linearMeterTotal;

        string LengthString = "Length";
        string WidthString = "Width";
        string thicknessString = "Thickness";
        string QtyString = "Qty";
        string QtyNameText = "Unit Quantity";
        string weightString = "Weight";
        string unitWeightTotalString = "Weight";

        string RollPath = "/Windows/Images/Roll.png";
        string RollDIMLPath = "/Windows/Images/RollDIML.png";
        string RollDIMWPath = "/Windows/Images/RollDIMW.png";

        string SheetPath = "/Windows/Images/Sheet.png";
        string SheetDIMLPath = "/Windows/Images/SheetDIML.png";
        string SheetDIMWPath = "/Windows/Images/SheetDIMW.png";

        string BagPath = "/Windows/Images/Bag.png";
        string LFTPath = "/Windows/Images/LFT.png";
        string CFSPath = "/Windows/Images/CFSOpen.png";
        string CFSDIMWPath = "/Windows/Images/CFSOpenDIMW.png";



        public WeightCalcWindow()
        {
            InitializeComponent();
            LoadTheme();

            TypeCombo.Items.Add("Sheet");
            TypeCombo.Items.Add("Bag");
            TypeCombo.Items.Add("CFS");
            TypeCombo.Items.Add("LFT");
            TypeCombo.Items.Add("Roll");
            TypeCombo.Text = "Sheet";

            LengthUnitCombo.Items.Add("m");
            LengthUnitCombo.Items.Add("mm");
            LengthUnitCombo.Items.Add("inch");
            LengthUnitCombo.Text = "mm";

            WidthUnitCombo.Items.Add("m");
            WidthUnitCombo.Items.Add("mm");
            WidthUnitCombo.Items.Add("inch");
            WidthUnitCombo.Text = "mm";

            ThicknessUnitCombo.Items.Add("um");
            ThicknessUnitCombo.Items.Add("gauge");
            ThicknessUnitCombo.Text = "um";

            SellingUnitCombo.Items.Add("Each");
            SellingUnitCombo.Items.Add("1000");
            SellingUnitCombo.Items.Add("Box");
            SellingUnitCombo.Items.Add("Roll");
            SellingUnitCombo.Text = "Each";

            exampleformulaupdate();
            MeterStackPanel.Opacity = 0.2;
            LLDPEBtn.Opacity = 0.2;

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
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else WindowState = WindowState.Maximized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoadTheme()
        {
            var CurrentUser = Environment.UserName;
            var folderpath = "C:\\Users\\" + CurrentUser + "\\AppData\\Roaming\\Matt K Programs\\Poly UK App";
            var filepath = "C:\\Users\\" + CurrentUser + "\\AppData\\Roaming\\Matt K Programs\\Poly UK App\\Theme.txt";


            if (!File.Exists(filepath))
            {
                Directory.CreateDirectory(folderpath);
                File.WriteAllText(filepath, "Light");
            }
            else if (File.Exists(filepath))
            {
                String themeSetting = File.ReadAllText(filepath).ToString();

                if (themeSetting == "Light")
                {
                    RollPath = "/Windows/Images/Roll.png";
                    RollDIMLPath = "/Windows/Images/RollDIML.png";
                    RollDIMWPath = "/Windows/Images/RollDIMW.png";

                    SheetPath = "/Windows/Images/Sheet.png";
                    SheetDIMLPath = "/Windows/Images/SheetDIML.png";
                    SheetDIMWPath = "/Windows/Images/SheetDIMW.png";

                    BagPath = "/Windows/Images/Bag.png";
                    LFTPath = "/Windows/Images/LFT.png";
                    CFSPath = "/Windows/Images/CFSOpen.png";
                    CFSDIMWPath = "/Windows/Images/CFSOpenDIMW.png";
                }
                if (themeSetting == "Dark")
                {
                    RollPath = "/Windows/Images/RollW.png";
                    RollDIMLPath = "/Windows/Images/RollDIMLW.png";
                    RollDIMWPath = "/Windows/Images/RollDIMWW.png";

                    SheetPath = "/Windows/Images/SheetW.png";
                    SheetDIMLPath = "/Windows/Images/SheetDIMLW.png";
                    SheetDIMWPath = "/Windows/Images/SheetDIMWW.png";

                    BagPath = "/Windows/Images/BagW.png";
                    LFTPath = "/Windows/Images/LFTW.png";
                    CFSPath = "/Windows/Images/CFSOpenW.png";
                    CFSDIMWPath = "/Windows/Images/CFSOpenDIMWW.png";
                }
            }
            return;

        }

        private void TypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String ProductTypeText = TypeCombo.SelectedItem.ToString();
            if (ProductTypeText == "Roll")
            {
                bagtube = 1;
                SheetPic.Source = new BitmapImage(new Uri(RollPath, UriKind.Relative));
                SheetDIMLPic.Source = new BitmapImage(new Uri(RollDIMLPath, UriKind.Relative));
                SheetDIMLPic.Margin = new Thickness(0);
                SheetDIMWPic.Source = new BitmapImage(new Uri(RollDIMWPath, UriKind.Relative));
                SheetDIMWPic.Margin = new Thickness(-10);
                DIMLText.Margin = new Thickness(90, -120, 0, 0);
                DIMWText.Margin = new Thickness(30, 190, 0, 0);
                TitleWidth.Text = "Width";
            }
            else if (ProductTypeText == "Sheet")
            {
                bagtube = 1;
                SheetPic.Source = new BitmapImage(new Uri(SheetPath, UriKind.Relative));
                SheetDIMLPic.Source = new BitmapImage(new Uri(SheetDIMLPath, UriKind.Relative));
                SheetDIMLPic.Margin = new Thickness(0);
                SheetDIMWPic.Source = new BitmapImage(new Uri(SheetDIMWPath, UriKind.Relative));
                SheetDIMWPic.Margin = new Thickness(0);
                DIMLText.Margin = new Thickness(90, -120, 0, 0);
                DIMWText.Margin = new Thickness(40, 180, 0, 0);
                TitleWidth.Text = "Width";
            }

            else if (ProductTypeText == "Bag")
            {
                bagtube = 2;
                SheetPic.Source = new BitmapImage(new Uri(BagPath, UriKind.Relative));
                SheetDIMLPic.Source = new BitmapImage(new Uri(SheetDIMLPath, UriKind.Relative));
                SheetDIMLPic.Margin = new Thickness(0);
                SheetDIMWPic.Source = new BitmapImage(new Uri(SheetDIMWPath, UriKind.Relative));
                SheetDIMWPic.Margin = new Thickness(10);
                DIMLText.Margin = new Thickness(90, -120, 0, 0);
                DIMWText.Margin = new Thickness(40, 180, 0, 0);
                TitleWidth.Text = "Width";
            }

            else if (ProductTypeText == "LFT")
            {
                bagtube = 2;
                SheetPic.Source = new BitmapImage(new Uri(LFTPath, UriKind.Relative));
                SheetDIMLPic.Source = new BitmapImage(new Uri(RollDIMLPath, UriKind.Relative));
                SheetDIMLPic.Margin = new Thickness(0);
                SheetDIMWPic.Source = new BitmapImage(new Uri(RollDIMWPath, UriKind.Relative));
                SheetDIMWPic.Margin = new Thickness(-15);
                DIMLText.Margin = new Thickness(90, -120, 0, 0);
                DIMWText.Margin = new Thickness(20, 200, 0, 0);
                TitleWidth.Text = "Width";
            }

            else if (ProductTypeText == "CFS")
            {
                bagtube = 1;
                SheetPic.Source = new BitmapImage(new Uri(CFSPath, UriKind.Relative));
                SheetDIMLPic.Source = new BitmapImage(new Uri(SheetDIMLPath, UriKind.Relative));
                SheetDIMLPic.Margin = new Thickness(-100, -40, 0, 0);
                SheetDIMWPic.Source = new BitmapImage(new Uri(CFSDIMWPath, UriKind.Relative));
                SheetDIMWPic.Margin = new Thickness(-30, -30, 0, 0);
                DIMLText.Margin = new Thickness(30, -180, 0, 0);
                DIMWText.Margin = new Thickness(40, 180, 0, 0);
                TitleWidth.Text = "Open Width";
            }

            WeightCalculationMethod();
            exampleformulaupdate();
        }

        private void SellingUnitCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SellingUnitCombo.SelectedItem == "Roll")
            {
                StackQuantityPanel.Visibility = Visibility.Visible;
                QtyPerText.Text = "Roll Quantity";
            }
            else if(SellingUnitCombo.SelectedItem == "Box")
            {

                StackQuantityPanel.Visibility = Visibility.Visible;
                QtyPerText.Text = "Box Quantity";
            }
            else
            {
                StackQuantityPanel.Visibility = Visibility.Hidden;
            }
            RollBoxQtySetter();
            WeightCalculationMethod();
            exampleformulaupdate();
        }

        private void LengthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MeterMeasuring)
            {
                DIMLText.Text = itemLength + "m";
            }
            else if (WeightMeasuring)
            {
                DIMLText.Text = LengthTextBox.Text + LengthUnitCombo.SelectedValue;
            }
            LengthSetterAndConverter();
            WeightCalculationMethod();
            exampleformulaupdate();
        }

        private void WidthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DIMWText.Text = WidthTextBox.Text + WidthUnitCombo.SelectedValue;
            WidthSetterAndConverter();
            WeightCalculationMethod();
            exampleformulaupdate();
        }

        private void LengthUnitCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MeterMeasuring)
            {
                DIMLText.Text = itemLength + "m";
            }
            else if (WeightMeasuring)
            {
                DIMLText.Text = LengthTextBox.Text + LengthUnitCombo.SelectedValue;
            }
            LengthSetterAndConverter();
            WeightCalculationMethod();
            exampleformulaupdate();
        }

        private void WidthUnitCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DIMWText.Text = WidthTextBox.Text + WidthUnitCombo.SelectedValue;
            WidthSetterAndConverter();
            WeightCalculationMethod();
            exampleformulaupdate();
        }

        private void LengthSetterAndConverter()
        {
            if (WeightMeasuring)
            {
                if (LengthTextBox.Text is not "")
                {
                    if (LengthUnitCombo.SelectedValue == "mm")
                    {
                        itemLength = Convert.ToDouble(LengthTextBox.Text) / 1000;
                    }
                    else if (LengthUnitCombo.SelectedValue == "inch")
                    {
                        itemLength = Convert.ToDouble(LengthTextBox.Text) / 39.3701;
                    }
                    else
                    {
                        itemLength = Convert.ToDouble(LengthTextBox.Text);
                    }
                }
            }
            else if (MeterMeasuring)
            {
                if (LengthTextBox.Text is not "")
                {
                    unitWeightTotal = Convert.ToDouble(LengthTextBox.Text);
                }
            }


        }

        private void WidthSetterAndConverter()
        {
            if(WidthTextBox.Text is not "")
            {
                if (WidthUnitCombo.SelectedValue == "mm")
                {
                    itemWidth = Convert.ToDouble(WidthTextBox.Text) / 1000;
                }
                else if (WidthUnitCombo.SelectedValue == "inch")
                {
                    itemWidth = Convert.ToDouble(WidthTextBox.Text) / 39.3701;
                }
                else
                {
                    itemWidth = Convert.ToDouble(WidthTextBox.Text);
                }
            }
        }

        private void ThicknessTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ThicknessTextBox.Text is not "")
            {
                if (ThicknessUnitCombo.SelectedValue == "um")
                {
                    itemthickness = Convert.ToDouble(ThicknessTextBox.Text) / 1000;
                }
                else if (ThicknessUnitCombo.SelectedValue == "gauge")
                {
                    itemthickness = (Convert.ToDouble(ThicknessTextBox.Text) * 0.254) / 1000;
                }
            }
            WeightCalculationMethod();
            exampleformulaupdate();
        }

        private void ThicknessUnitCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThicknessTextBox.Text is not "")
            {
                if (ThicknessUnitCombo.SelectedValue == "um")
                {
                    itemthickness = Convert.ToDouble(ThicknessTextBox.Text) / 1000;
                }
                else if (ThicknessUnitCombo.SelectedValue == "gauge")
                {
                    itemthickness = (Convert.ToDouble(ThicknessTextBox.Text) * 0.254) / 1000;
                }
            }
            WeightCalculationMethod();
            exampleformulaupdate();
        }

        private void QtyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RollBoxQtySetter();
            WeightCalculationMethod();
            exampleformulaupdate();
        }

        private void unitQtyBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TotalQtySetter();
            WeightCalculationMethod();
            exampleformulaupdate();
        }

        private void RollBoxQtySetter()
        {
            if (StackQuantityPanel.Visibility == Visibility.Hidden && SellingUnitCombo.SelectedItem == "1000")
            {
                bagboxQuantity = 1000;
            }
            else
            {
                if (StackQuantityPanel.Visibility == Visibility.Visible && QtyTextBox.Text is not "")
                {
                    bagboxQuantity = Convert.ToDouble(QtyTextBox.Text);
                }
                else
                {
                    bagboxQuantity = 1;
                }
            }

        }

        private void TotalQtySetter()
        {
            if (unitQtyBox.Text is not "")
            {
                unitQuantity = Convert.ToDouble(unitQtyBox.Text);
            }
            else
            {
                unitQuantity = 1;
            }
        }

        private void WeightCalculationMethod()
        {
            if (WeightMeasuring)
            {
                unitWeightTotal = (itemLength * itemWidth * itemthickness * relativeDensity * bagtube) * bagboxQuantity;
                unitWeightAnswer.Text = Math.Round(unitWeightTotal, 2).ToString() + "kg";
                totalWeightTotal = ((itemLength * itemWidth * itemthickness * relativeDensity * bagtube) * bagboxQuantity) * unitQuantity;
                totalWeightAnswer.Text = Math.Round(totalWeightTotal, 2).ToString() + "kg";
            }
            else if (MeterMeasuring)
            {
                linearMeterTotal = unitWeightTotal / bagboxQuantity / bagtube / relativeDensity / itemthickness / itemWidth;
                if(double.IsFinite(linearMeterTotal))
                {
                    TotalMeter.Text = Math.Round(linearMeterTotal, 2).ToString() + "m";
                    DIMLText.Text = Math.Round(linearMeterTotal, 2).ToString() + "m";
                }
                else
                {
                    TotalMeter.Text = "0m";
                    DIMLText.Text = "0m";
                }
            }


        }

        private void exampleformulaupdate()
        {
            if(WeightMeasuring)
            {
                if (itemLength is not 0)
                {
                    LengthString = itemLength.ToString();
                }
                if (itemWidth is not 0)
                {
                    WidthString = itemWidth.ToString();
                }
                if (itemthickness is not 0)
                {
                    thicknessString = itemthickness.ToString();
                }
                if (bagboxQuantity is not 0)
                {
                    QtyString = bagboxQuantity.ToString();
                }
                if (unitWeightTotal is not 0)
                {
                    weightString = Math.Round(unitWeightTotal, 3).ToString();
                }

                if (StackQuantityPanel.Visibility == Visibility.Hidden)
                {
                    QtyNameText = "Unit Quantity";
                }
                else if (StackQuantityPanel.Visibility == Visibility.Visible)
                {
                    QtyNameText = QtyPerText.Text;
                }

                String ProductTypeText = TypeCombo.SelectedItem.ToString();

                if (ProductTypeText == "Roll" || ProductTypeText == "Sheet" || ProductTypeText == "CFS")
                {
                    FormulaExampleTxt.Text = LengthString + "(m) x " + WidthString + "(m) x " + thicknessString + "(mm) x " + relativeDensity.ToString() + " (r.d.)" + " x " + QtyString + " (" + QtyNameText + ") = " + weightString + "(kg)";
                }
                else if (ProductTypeText == "Bag" || ProductTypeText == "LFT")
                {
                    FormulaExampleTxt.Text = LengthString + "(m) x " + WidthString + "(m) x " + thicknessString + "(mm) x " + relativeDensity.ToString() + " (r.d.)" + " x " + "2 (bag/tube)" + " x " + QtyString + " (" + QtyNameText + ") = " + weightString + "(kg)";
                }
            }
            else if (MeterMeasuring)
            {
                if (itemWidth is not 0)
                {
                    WidthString = itemWidth.ToString();
                }
                if (itemthickness is not 0)
                {
                    thicknessString = itemthickness.ToString();
                }
                if (unitWeightTotal is not 0)
                {
                    unitWeightTotalString = unitWeightTotal.ToString();
                }

                String ProductTypeText = TypeCombo.SelectedItem.ToString();

                if(ProductTypeText == "LFT")
                {
                    FormulaExampleTxt.Text = unitWeightTotalString + "(kg) / " + relativeDensity + " (r.d.) / 2 (bag/tube) / " + thicknessString + "(mm) / " + WidthString + "(m) = " + Math.Round(linearMeterTotal, 2).ToString() + "(m)";
                }
                else
                {
                    FormulaExampleTxt.Text = unitWeightTotalString + "(kg) / " + relativeDensity + " (r.d.) / " + thicknessString + "(mm) / " + WidthString + "(m) = " + Math.Round(linearMeterTotal, 2).ToString() + "(m)";
                }

            }


        }

        private void WeightBtn_Click(object sender, RoutedEventArgs e)
        {
            WeightMeasuring = true;
            MeterMeasuring = false;
            LengthTextTitle.Text = "Length";
            LengthTextBox.Text = "";
            LengthUnitCombo.Items.Clear();
            LengthUnitCombo.Items.Add("m");
            LengthUnitCombo.Items.Add("mm");
            LengthUnitCombo.Items.Add("inch");
            LengthUnitCombo.Text = "mm";
            TypeCombo.Text = "Sheet";
            SellingUnitCombo.Text = "Each";
            WidthTextBox.Text = "";
            ThicknessTextBox.Text = "";
            QtyTextBox.Text = "";
            unitQtyBox.Text = "";
            linearMeterTotal = 0.00;
            TotalMeter.Text = Math.Round(linearMeterTotal, 2).ToString() + "m";
            LengthString = "Length";
            WidthString = "Width";
            thicknessString = "Thickness";
            QtyString = "Qty";
            QtyNameText = "Unit Quantity";
            weightString = "Weight";
            unitWeightTotalString = "Weight";
            itemLength = 0;
            itemWidth = 0;
            itemthickness = 0;
            bagboxQuantity = 1;
            unitQuantity = 1;
            unitWeightTotal = 0;
            totalWeightTotal = 0;
            linearMeterTotal = 0;

            TotalWeightStack.Opacity = 1;
            MeterStackPanel.Opacity = 0.2;


            WeightCalculationMethod();
            exampleformulaupdate();
        }

        private void MeterBtn_Click(object sender, RoutedEventArgs e)
        {
            WeightMeasuring = false;
            MeterMeasuring = true;
            LengthTextTitle.Text = "Weight";
            LengthTextBox.Text = "";
            LengthUnitCombo.Items.Clear();
            LengthUnitCombo.Items.Add("kg");
            LengthUnitCombo.Text = "kg";
            DIMLText.Text = "m";
            TypeCombo.Text = "Roll";
            SellingUnitCombo.Text = "Roll";
            WidthTextBox.Text = "";
            ThicknessTextBox.Text = "";
            QtyTextBox.Text = "1";
            unitQtyBox.Text = "1";
            unitWeightTotal = 0.00;
            unitWeightAnswer.Text = Math.Round(unitWeightTotal, 2).ToString() + "kg";
            totalWeightTotal = 0.00;
            totalWeightAnswer.Text = Math.Round(totalWeightTotal, 2).ToString() + "kg";
            LengthString = "Length";
            WidthString = "Width";
            thicknessString = "Thickness";
            QtyString = "Qty";
            QtyNameText = "Unit Quantity";
            weightString = "Weight";
            unitWeightTotalString = "Weight";
            itemLength = 0;
            itemWidth = 0;
            itemthickness = 0;
            bagboxQuantity = 1;
            unitQuantity = 1;
            unitWeightTotal = 0;
            totalWeightTotal = 0;
            linearMeterTotal = 0;

            TotalWeightStack.Opacity = 0.2;
            MeterStackPanel.Opacity = 1.0;

            WeightCalculationMethod();
            exampleformulaupdate();
        }

        private void LLDPEBtn_Click(object sender, RoutedEventArgs e)
        {
            LLDPEBtn.Opacity = 1;
            LDPEBtn.Opacity = 0.2;
            relativeDensity = 0.92;
            exampleformulaupdate();
            WeightCalculationMethod();
        }

        private void LDPEBtn_Click(object sender, RoutedEventArgs e)
        {
            LDPEBtn.Opacity = 1;
            LLDPEBtn.Opacity = 0.2;
            relativeDensity = 0.923;
            exampleformulaupdate();
            WeightCalculationMethod();
        }
    }
}
