using System;
using System.Collections.Generic;
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
    /// Interaction logic for MarginCalcWindow.xaml
    /// </summary>
    public partial class MarginCalcWindow : Window
    {

        ///Main Variables
        double CostPrice;
        double SalesPrice;
        double CarriageCost;
        double CarriageCharge;
        double Quantity;
        double NewQuantity;
        double NewCostPrice;
        double NewMarginCostPrice;
        double NewSalesPrice;
        double NewCarriageCost;
        double NewCarriageCharge;
        double NewMarginSalesPrice;
        double CurrentMargin;
        double TargetMargin;


        public MarginCalcWindow()
        {
            InitializeComponent();
            CurrentMarginExplain();
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
            Close();
        }

        private void OriginalMarginCalc()
        {
            //Enter cost
            if(CostPriceTextBox.Text != "" && QtyTextBox.Text != "")
            {
                CostPrice = Convert.ToDouble(CostPriceTextBox.Text) * Convert.ToDouble(QtyTextBox.Text);
            }
            //Enter Sales Price
            if(SalePriceTextBox.Text != "" && QtyTextBox.Text != "")
            {
                SalesPrice = Convert.ToDouble(SalePriceTextBox.Text) * Convert.ToDouble(QtyTextBox.Text);
            }
            if(CarriageCostTextBox.Text != "")
            {
                CarriageCost = Convert.ToDouble(CarriageCostTextBox.Text);
            }
            if(CarriageChargeTextBox.Text != "")
            {
                CarriageCharge = Convert.ToDouble(CarriageChargeTextBox.Text);
            }
            if(QtyTextBox.Text != "")
            {
                Quantity = Convert.ToDouble(QtyTextBox.Text);
                if(CostPriceTextBox.Text != "")
                {
                    CostPrice = Convert.ToDouble(CostPriceTextBox.Text) * Convert.ToDouble(QtyTextBox.Text);
                }
                if(SalePriceTextBox.Text != "")
                {
                    SalesPrice = Convert.ToDouble(SalePriceTextBox.Text) * Convert.ToDouble(QtyTextBox.Text);
                }
            }

            if (CostPrice != null && SalesPrice != null && CarriageCharge != null && CarriageCharge != null && Quantity != null)
            {
                CurrentMargin = Math.Round((1 - ((CostPrice + CarriageCost) / (SalesPrice + CarriageCharge))) * 100,2);
            }
            FinalMarginCalcText.Text = CurrentMargin.ToString() + " %";
            CurrentMarginNewCostText.Text = CurrentMargin.ToString() + " %";

        }

        private void NewMarginCalc()
        {
            if(NewCostPriceTextBox.Text != "" && NewQtyTextBox.Text != "")
            {
                NewMarginCostPrice = Convert.ToDouble(NewCostPriceTextBox.Text) * Convert.ToDouble(NewQtyTextBox.Text);
            }
            if(TargetMarginTextBox.Text != "")
            {
                TargetMargin = Convert.ToDouble(TargetMarginTextBox.Text) / 100;
            }
            if(NewCarriageCostTextBox.Text != "")
            {
                NewCarriageCost = Convert.ToDouble(NewCarriageCostTextBox.Text);
            }
            if(NewCarriageChargeTextBox.Text != "")
            {
                NewCarriageCharge = Convert.ToDouble(NewCarriageChargeTextBox.Text);
            }
            if(NewQtyTextBox.Text != "")
            {
                NewQuantity = Convert.ToDouble(NewQtyTextBox.Text);
                if(NewCostPriceTextBox.Text != "")
                {
                    NewCostPrice = Convert.ToDouble(NewCostPriceTextBox.Text) * Convert.ToDouble(NewQtyTextBox.Text);
                }
            }
            if(NewCostPrice != null && TargetMargin != null && NewCarriageCost != null && NewCarriageCharge != null && NewQuantity != null)
            {
                NewMarginSalesPrice = Math.Round((((NewMarginCostPrice + NewCarriageCost) / (1 - TargetMargin)) - NewCarriageCharge) / NewQuantity, 2);
                    NewMarginSalesPriceValueText.Text = "£" + NewMarginSalesPrice.ToString();
            }
        }

        private void NewCostPriceCalc()
        {
            if (CurrentMargin != null && NewCostTextBox.Text != "")
            {
                NewCostPrice = Convert.ToDouble(NewCostTextBox.Text);
                NewSalesPrice = Math.Round(((((NewCostPrice * Quantity) + CarriageCost) / (1 - (CurrentMargin / 100))) - CarriageCharge) / Quantity, 2);
                NewSellingPriceCalcText.Text = "£" + NewSalesPrice.ToString();
            }
        }

        private void CurrentMarginExplain()
        {
            CurrentMarginTextExp.Text = "";
            CurrentMarginTextExp.Text = "This is a very long test to make sure it carries over line to line";
        }

        private void CostPriceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OriginalMarginCalc();
            NewCostPriceCalc();
        }

        private void CarriageCostTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OriginalMarginCalc();
            NewCostPriceCalc();
        }

        private void SalePriceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OriginalMarginCalc();
            NewCostPriceCalc();
        }

        private void CarriageChargeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OriginalMarginCalc();
            NewCostPriceCalc();
        }

        private void QtyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OriginalMarginCalc();
            NewCostPriceCalc();
        }

        private void NewCostTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NewCostPriceCalc();
        }

        private void NewCostPriceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NewMarginCalc();
        }

        private void TargetMarginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NewMarginCalc();
        }

        private void NewCarriageCostTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NewMarginCalc();
        }

        private void NewCarriageChargeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NewMarginCalc();
        }

        private void NewQtyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NewMarginCalc();
        }
    }
}
