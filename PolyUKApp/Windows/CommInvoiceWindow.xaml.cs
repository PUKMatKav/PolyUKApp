using Microsoft.Data.SqlClient;
using Mysqlx.Connection;
using Mysqlx.Crud;
using PolyUKApp.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static PolyUKApp.Windows.CallTimeWindow;
using MessageBox = System.Windows.MessageBox;

namespace PolyUKApp.Windows
{
    /// <summary>
    /// Interaction logic for CommInvoiceWindow.xaml
    /// </summary>
    public partial class CommInvoiceWindow : Window
    {
        static readonly int MyHotKeyId = 0x3000;
        static readonly int WM_HOTKEY = 0x312;



        public CommInvoiceWindow()
        {
            InitializeComponent();

        }

        internal static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern bool RegisterHotKey(IntPtr windowHandle, int hotkeyId, uint modifierKeys, uint virtualKey);

            [DllImport("user32.dll")]
            public static extern bool UnregisterHotKey(IntPtr windowHandle, int hotkeyId);
        }

        void InitializeHook()
        {
            var windowHelper = new WindowInteropHelper(this);
            var windowSource = HwndSource.FromHwnd(windowHelper.Handle);

            windowSource.AddHook(MessagePumpHook);
        }

        IntPtr MessagePumpHook(IntPtr handle, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                if ((int)wParam == MyHotKeyId && TxtBxSearch.Text.Length > 5 && TxtBxSearch.Visibility is Visibility.Visible)
                {
                    PUKLogo.Visibility = Visibility.Visible;
                    GeneratedBorder.Visibility = Visibility.Visible;
                    SearchBorder.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    SearchBorder.Width = 120;
                    SearchBorder.CornerRadius = new CornerRadius(10, 0, 0, 10);
                    SearchTextBoxBackground.Visibility = Visibility.Hidden;
                    TxtBxSearch.Visibility = Visibility.Hidden;
                    BtnGenCI.Visibility = Visibility.Hidden;

                    var currentdate = (DateTime.Now).ToString().Substring(0, 10);

                    InvFromText.AppendText("XI903824828000" + "\r" + "Polythene UK Ltd" + "\r" + "31c Avenue 1" + "\r" + "Station Lane" + "\r" + "Witney" + "\r" + "OX28 4XZ" + "\r" + "0845 643 1601");
                    OriginLOC.Text = "UK";
                    InvDate.Text = currentdate;

                    OrderDataSQL();
                    DetailsSQL();
                    ReadWriteCINumber();

                    MessageBox.Show("Please check all information is correct and filled in");

                    handled = true;
                }
            }

            return IntPtr.Zero;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            InitializeHook();
            InitializeHotKey();
        }

        void InitializeHotKey()
        {
            var windowHelper = new WindowInteropHelper(this);

            // Specify modifiers such as SHIFT, ALT, CONTROL, and WIN.
            // Remember to use the bit-wise OR operator (|) to join multiple modifiers together.
            uint modifiers = (uint)ModifierKeys.None;

            // We need to convert the WPF Key enumeration into a virtual key for the Win32 API!
            uint virtualKey = (uint)KeyInterop.VirtualKeyFromKey(Key.Enter);
            NativeMethods.RegisterHotKey(windowHelper.Handle, MyHotKeyId, modifiers, virtualKey);


        }

        void UninitializeHotKey()
        {
            var windowHelper = new WindowInteropHelper(this);
            NativeMethods.UnregisterHotKey(windowHelper.Handle, MyHotKeyId);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            UninitializeHotKey();
            Close();
        }



        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            //hide some stuff
            BtnClose.Visibility = Visibility.Hidden;
            BtnPrint.Visibility = Visibility.Hidden;
            BtnResetCI.Visibility = Visibility.Hidden;
            SearchBorder.Visibility = Visibility.Hidden;
            DragHandle.Visibility = Visibility.Hidden;
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
            DragHandle.Visibility = Visibility.Visible;
            //dialog.ShowDialog();

            //Update CI number on Print Press
            string CurrentUser = Globals.Username;
            String filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\data\\CommInvNumber.txt";
            var ComInvNum = Convert.ToDouble(File.ReadAllText(filepath)) + 1;
            File.WriteAllText(filepath, ComInvNum.ToString());

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
            ReadWriteCINumber();

            MessageBox.Show("Please check all information is correct and filled in");
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
            CertTextBlock.Text = string.Empty;
            SubTotTextBlock.Text = string.Empty;
            VATTextBlock.Text = string.Empty;
            TotTextBlock.Text = string.Empty;
            InvNumber.Text = string.Empty;
            DataGridCI.ItemsSource = null;
            CurrencyTextBlock.Text = string.Empty;
        }

        public void OrderDataSQL()
        {
            var connectionString = DataAccess.GlobalSQL.Connection;
            DataTable OrderTable = new DataTable();
            DataTable InvoiceItemTable = new DataTable();

            //Order to generate from

            var OrderNum = TxtBxSearch.Text;

            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.OrderCIQuery;
                var queryStatement2 = DataAccess.GlabalSQLQueries.OrderCICodeQuery;

                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    var OrderTest = TxtBxSearch.Text;
                    if (OrderTest.ToString().Count() == 6)
                    {
                        _cmd.Parameters.AddWithValue("OrderNum", "0000" + OrderNum);
                    }
                    else
                    {
                        _cmd.Parameters.AddWithValue("OrderNum", OrderNum);
                    }
                    _dap.Fill(OrderTable);
                }
                using (SqlCommand _cmd2 = new SqlCommand(queryStatement2, _con))
                {
                    SqlDataAdapter _dap2 = new SqlDataAdapter(_cmd2);
                    _dap2.Fill(InvoiceItemTable);
                }
            }

            //DataTable UniqueItemTable = InvoiceItemTable.DefaultView.ToTable(true);

            OrderTable.PrimaryKey = new DataColumn[] { OrderTable.Columns[0] };
            InvoiceItemTable.PrimaryKey = new DataColumn[] { InvoiceItemTable.Columns[0] };

            OrderTable.Merge(InvoiceItemTable);

            DataTable ItemTable = new DataTable();
            ItemTable.Columns.Add("Code");
            ItemTable.Columns.Add("Description");
            ItemTable.Columns.Add("Qty");
            ItemTable.Columns.Add("Unit");
            ItemTable.Columns.Add("HSCode");
            ItemTable.Columns.Add("Weight");
            ItemTable.Columns.Add("Grosskg");
            ItemTable.Columns.Add("Price");
            ItemTable.Columns.Add("Total");


            foreach (DataRow MergeRow in OrderTable.Rows)
            {
                if (MergeRow[5] is DBNull)
                {
                    MergeRow.Delete();
                   
                }
            }
            OrderTable.AcceptChanges();

            foreach (DataRow Row in OrderTable.Rows)
            {
                if (Row[3].ToString() == "1000")
                {
                    Row[30] = Convert.ToDouble(Row["Weight"]) / 1000;
                    Row[3] = "Each";
                    Row[2] = Convert.ToDouble(Row["UnitSellingPrice"]) / 1000;
                    Row[1] = Convert.ToDouble(Row["LineQuantity"]) * 1000;

                }
                OrderTable.AcceptChanges();
                var TotalPrice = Convert.ToDouble(Row["UnitSellingPrice"]) * Convert.ToDouble(Row["LineQuantity"]);
                var NetWeight = Math.Round(Convert.ToDouble(Row["Weight"]) * Convert.ToDouble(Row["LineQuantity"]),2);
                var GrossWeight = NetWeight + 20;
                if (NetWeight > 0)
                {
                    ItemTable.Rows.Add(Row["ItemCode"], Row["ItemDescription"], Row["LineQuantity"], Row["SellingUnitDescription"], "3920102899", NetWeight, GrossWeight, Row["UnitSellingPrice"], Math.Round(TotalPrice, 2));
                }
                else
                {
                    ItemTable.Rows.Add(Row["ItemCode"], Row["ItemDescription"], Row["LineQuantity"], Row["SellingUnitDescription"], "3920102899", "Please Enter", "Please Enter", Row["UnitSellingPrice"], Math.Round(TotalPrice, 2));
                }

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
                    var OrderTest = TxtBxSearch.Text;
                    if (OrderTest.ToString().Count() == 6)
                    {
                        _cmd.Parameters.AddWithValue("OrderNum", "0000" + OrderNum);
                    }
                    else
                    {
                        _cmd.Parameters.AddWithValue("OrderNum", OrderNum);
                    }
                    _dap.Fill(InvoiceAddTable);
                }

                    _con.Close();

                foreach (DataRow Row in InvoiceAddTable.Rows)
                {
                    String VATNum = Row["TaxRegistrationNumber"].ToString();
                    String CountryCode = Row["Code"].ToString();
                    if (CountryCode == "GB" && VATNum.Substring(0,3) == "GB ")
                    {
                        InvToText.AppendText("GB" + VATNum.Substring(3) + "000" + "\r");
                    }
                    else if (CountryCode == "GB" && VATNum.Substring(0, 2) == "GB")
                    {
                        InvToText.AppendText("GB" + VATNum.Substring(2) + "000" + "\r");
                    }
                    else if (CountryCode == "GB")
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
                    String ItemCodeBRC = Row["ItemCode"].ToString().Substring(0,3);
                    if (ItemCodeBRC == "BRC")
                    {
                        CertTextBlock.Text = "BRC";
                    }
                    else
                    {
                        CertTextBlock.Text = "N/A";
                    }

                    String SubTotalVal = Row["SubtotalGoodsValue"].ToString();
                    SubTotTextBlock.Text = SubTotalVal;
                    String VATTotalVal = Row["TotalTaxValue"].ToString();
                    VATTextBlock.Text = VATTotalVal;
                    String GrossTotalVal = Row["TotalGrossValue"].ToString();
                    TotTextBlock.Text = GrossTotalVal;

                    String CurrencySymbol = Row["Symbol"].ToString();
                    if (CurrencySymbol == "£")
                    {
                        CurrencyTextBlock.Text = "All Currency listed in £ (Pounds)";
                    }
                    else
                    {
                        CurrencyTextBlock.Text = "All Currency listed in € (Euros)";
                    }
                }

            }

        }

        public void ReadWriteCINumber()
        {
            string CurrentUser = Globals.Username;
            String filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\data\\CommInvNumber.txt";
            var ComInvNum = Convert.ToDouble(File.ReadAllText(filepath));

            var LeadZero = ComInvNum.ToString().Length;
            var ZeroCount = 8 - LeadZero;
            String LeadZeroString = "";
            for (int i = ZeroCount; i > 0; i--)
            {
                LeadZeroString += "0";
            }
            InvNumber.Text = LeadZeroString + ComInvNum.ToString();
        }

        private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void DragHandle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
