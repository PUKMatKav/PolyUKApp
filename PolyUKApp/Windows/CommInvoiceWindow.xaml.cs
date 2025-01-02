using Microsoft.Data.SqlClient;
using Mysqlx.Connection;
using PolyUKApp.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
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
    /// Interaction logic for CommInvoiceWindow.xaml
    /// </summary>
    public partial class CommInvoiceWindow : Window
    {
        public CommInvoiceWindow()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }



        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            //hide some stuff
            BtnClose.Visibility = Visibility.Hidden;
            BtnPrint.Visibility = Visibility.Hidden;
            BtnResetCI.Visibility = Visibility.Hidden;
            SearchBorder.Visibility = Visibility.Hidden;
            System.Windows.Controls.PrintDialog dialog = new System.Windows.Controls.PrintDialog();
            if (dialog.ShowDialog() == true)
            {

                //get printer capabilities
                System.Printing.PrintCapabilities capabilities = dialog.PrintQueue.GetPrintCapabilities(dialog.PrintTicket);

                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / this.ActualWidth, capabilities.PageImageableArea.ExtentHeight / this.ActualHeight);
                this.LayoutTransform = new ScaleTransform(scale, scale);
                System.Windows.Size sz = new System.Windows.Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
                this.Measure(sz);
                this.Arrange(new Rect(new System.Windows.Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));
                dialog.PrintVisual(this, "Info Grid");
            }
            //reset window to original size
            double WindowHeight = 1019;
            double WindowFinalHeight = 1020;
            double WindowWidth = 800;
            CommInvWindow.Height = WindowHeight;
            CommInvWindow.Width = WindowWidth;
            CommInvWindow.Height = WindowFinalHeight;
            //show some stuff
            BtnClose.Visibility = Visibility.Visible;
            BtnPrint.Visibility = Visibility.Visible;
            BtnResetCI.Visibility = Visibility.Visible;
            SearchBorder.Visibility = Visibility.Visible;
            //dialog.ShowDialog();

        }

        private void BtnGenCI_Click(object sender, RoutedEventArgs e)
        {
            PUKLogo.Visibility = Visibility.Visible;
            GeneratedBorder.Visibility = Visibility.Visible;
            SearchBorder.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            SearchBorder.Width = 120;
            SearchBorder.CornerRadius = new CornerRadius(10, 0, 0, 10);
            SearchTextBoxBackground.Visibility = Visibility.Hidden;
            TxtBxSearch.Visibility = Visibility.Hidden;
            BtnGenCI.Visibility = Visibility.Hidden;

            var currentdate = (DateTime.Now).ToString().Substring(0,10);

            InvFromText.AppendText("XI903824828000" + "\r" + "Polythene UK Ltd" + "\r" + "31c Avenue 1" + "\r" + "Station Lane" + "\r" + "Witney" + "\r" + "OX28 4XZ" + "\r" + "0845 643 1601");
            OriginLOC.Text = "UK";
            InvDate.Text = currentdate;

            OrderDataSQL();
            DetailsSQL();
        }

        private void BtnResetCI_Click(object sender, RoutedEventArgs e)
        {
            PUKLogo.Visibility = Visibility.Hidden;
            GeneratedBorder.Visibility = Visibility.Hidden;
            SearchBorder.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            SearchBorder.Width = Double.NaN;
            SearchBorder.CornerRadius = new CornerRadius(0, 0, 0, 0);
            SearchTextBoxBackground.Visibility = Visibility.Visible;
            TxtBxSearch.Visibility = Visibility.Visible;
            BtnGenCI.Visibility = Visibility.Visible;

            TxtBxSearch.Text = "";
            OriginLOC.Text = string.Empty;
            InvDate.Text = string.Empty;
            InvFromText.Document.Blocks.Clear();
            InvToText.Document.Blocks.Clear();
            DelToText.Document.Blocks.Clear();
            OrderNumberTextBlock.Text = string.Empty;
            CusPOTextBlock.Text = string.Empty;
            TermsTextBlock.Text = string.Empty;
            INCOTERMSTextBlock.Text = string.Empty;
        }

        public void OrderDataSQL()
        {
            var connectionString = DataAccess.GlobalSQL.Connection;
            DataTable OrderTable = new DataTable();

            //Order to generate from
            var OrderNum = TxtBxSearch.Text;

            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.OrderCIQuery;

                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    _cmd.Parameters.AddWithValue("OrderNum", OrderNum);
                    _dap.Fill(OrderTable);
                }
            }

            DataTable ItemTable = new DataTable();
            ItemTable.Columns.Add("Code");
            ItemTable.Columns.Add("Description");
            ItemTable.Columns.Add("Qty");
            ItemTable.Columns.Add("Unit");
            ItemTable.Columns.Add("HS Code");
            ItemTable.Columns.Add("Net Weight (kg)");
            ItemTable.Columns.Add("Gross Weight (kg)");
            ItemTable.Columns.Add("Price");
            ItemTable.Columns.Add("Total");
            foreach (DataRow Row in OrderTable.Rows)
            {
                var TotalPrice = Convert.ToDouble(Row["UnitSellingPrice"]) * Convert.ToDouble(Row["LineQuantity"]);
                ItemTable.Rows.Add(Row["ItemCode"], Row["ItemDescription"], Row["LineQuantity"], Row["SellingUnitDescription"], "3920102899", "Net Weight", "Gross weight", Row["UnitSellingPrice"], TotalPrice.ToString());
            }

            DataGridCI.ItemsSource = ItemTable.DefaultView;
        }

        public void DetailsSQL()
        {
            var connectionString = DataAccess.GlobalSQL.Connection;
            DataTable InvoiceAddTable = new DataTable();

            //Order to generate from
            var OrderNum = TxtBxSearch.Text;
            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.OrderCIQuery;

                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    _cmd.Parameters.AddWithValue("OrderNum", OrderNum);
                    _dap.Fill(InvoiceAddTable);
                }
                _con.Close();

                foreach (DataRow Row in InvoiceAddTable.Rows)
                {
                    String VATNum = Row["TaxRegistrationNumber"].ToString();
                    String CountryCode = Row["Code"].ToString();
                    if (CountryCode == "GB")
                    {
                        InvToText.AppendText("GB" + VATNum + "000" + "\r");
                    }
                    else
                    {
                        InvToText.AppendText(CountryCode + VATNum + "\r");
                    }
                    String COName = Row["CustomerAccountName"].ToString();
                    InvToText.AppendText(COName + "\r");
                    String AddLine1 = Row["AddressLine1"].ToString();
                    if (AddLine1 == "") { }
                    else
                    {
                        InvToText.AppendText(AddLine1 + "\r");
                    }
                    String AddLine2 = Row["AddressLine2"].ToString();
                    if (AddLine2 == "") { }
                    else
                    {
                        InvToText.AppendText(AddLine2 + "\r");
                    }
                    String AddLine3 = Row["AddressLine3"].ToString();
                    if (AddLine3 == "") { }
                    else
                    {
                        InvToText.AppendText(AddLine3 + "\r");
                    }
                    String AddLine4 = Row["AddressLine4"].ToString();
                    if (AddLine4 == "") { }
                    else
                    {
                        InvToText.AppendText(AddLine4 + "\r");
                    }
                    String InvPostcode = Row["InvPostCode"].ToString();
                    InvToText.AppendText(InvPostcode + "\r");

                    object UseInvAddress = Row["UseInvoiceAddress"];
                    if (UseInvAddress is true)
                    {
                        DelToText.AppendText("As Per Invoice Address");
                    }
                    else 
                    {
                        String DelCOName = Row["PostalName"].ToString();
                        if (DelCOName == "") { }
                        else
                        {
                            DelToText.AppendText(DelCOName + "\r");
                        }
                        String DelAdd1 = Row["DelAdd1"].ToString();
                        if (DelAdd1 == "") { }
                        else
                        {
                            DelToText.AppendText(DelAdd1 + "\r");
                        }
                        String DelAdd2 = Row["DelAdd2"].ToString();
                        if (DelAdd2 == "") { }
                        else
                        {
                            DelToText.AppendText(DelAdd2 + "\r");
                        }
                        String DelAdd3 = Row["DelAdd3"].ToString();
                        if (DelAdd3 == "") { }
                        else
                        {
                            DelToText.AppendText(DelAdd3 + "\r");
                        }
                        String DelPostcode = Row["DelPostcode"].ToString();
                        if (DelPostcode == "") { }
                        else
                        {
                            DelToText.AppendText(DelPostcode + "\r");
                        }
                    }

                    String PUKOrderNum = Row["DocumentNo"].ToString();
                    OrderNumberTextBlock.Text = PUKOrderNum;
                    String CustomerPONum = Row["CustomerDocumentNo"].ToString();
                    CusPOTextBlock.Text = CustomerPONum;
                    String CusTerms = Row["TradingTerms"].ToString();
                    TermsTextBlock.Text = CusTerms;
                    INCOTERMSTextBlock.Text = "DAP";


                }


            }

        }
    }
}
