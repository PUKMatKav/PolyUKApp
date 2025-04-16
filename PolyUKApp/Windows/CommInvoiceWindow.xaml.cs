using Google.Protobuf.WellKnownTypes;
using Microsoft.Data.SqlClient;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.VisualBasic;
using Mysqlx.Connection;
using Mysqlx.Crud;
using PolyUKApp.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
using System.Windows.Forms.VisualStyles;
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
                    DetailsRecall();

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
            SaveDraft();

            //hide some stuff
            BtnClose.Visibility = Visibility.Hidden;
            BtnPrint.Visibility = Visibility.Hidden;
            BtnSaveCI.Visibility = Visibility.Hidden;
            BtnResetCI.Visibility = Visibility.Hidden;
            SearchBorder.Visibility = Visibility.Hidden;
            DragHandle.Visibility = Visibility.Hidden;
            

            //set light theme
            AppTheme.ChangeTheme(new Uri("Theme/AppLight.xaml", UriKind.Relative));


            //set window to size
            double AWindowHeight = 1019;
            double AWindowFinalHeight = 1020;
            double AWindowWidth = 800;
            CommInvWindow.Height = AWindowHeight;
            CommInvWindow.Width = AWindowWidth;
            CommInvWindow.Height = AWindowFinalHeight;

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
            BtnSaveCI.Visibility = Visibility.Visible;
            BtnResetCI.Visibility = Visibility.Visible;
            SearchBorder.Visibility = Visibility.Visible;
            DragHandle.Visibility = Visibility.Visible;
            //dialog.ShowDialog();

            //set original theme
            LoadTheme();

            //Update CI number on Print Press
            //string CurrentUser = Globals.Username;
            //String filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\data\\CommInvNumber.txt";
            //var ComInvNum = Convert.ToDouble(File.ReadAllText(filepath)) + 1;
            //File.WriteAllText(filepath, ComInvNum.ToString());
            

        }

        private void BtnGenCI_Click(object sender, RoutedEventArgs e)
        {
            DetailsRecall();


        }

        private void BtnResetCI_Click(object sender, RoutedEventArgs e)
        {
            PUKLogo.Visibility = Visibility.Hidden;
            GeneratedBorder.Visibility = Visibility.Hidden;
            BtnSaveCI.Visibility = Visibility.Hidden;
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
            INCOTERMSTextBlock.Document.Blocks.Clear();
            CertTextBlock.Text = string.Empty;
            SubTotTextBlock.Text = string.Empty;
            VATTextBlock.Text = string.Empty;
            TotTextBlock.Text = string.Empty;
            InvNumber.Document.Blocks.Clear();
            ContactEmailTextBlock.Document.Blocks.Clear();
            PalletsTextBlock.Document.Blocks.Clear();
            PalletsTextBlock.AppendText("PLEASE ENTER");
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
                    Row[31] = Convert.ToDouble(Row["Weight"]) / 1000;
                    Row[3] = "Each";
                    Row[2] = Convert.ToDouble(Row["UnitSellingPrice"]) / 1000;
                    Row[1] = Convert.ToDouble(Row["LineQuantity"]) * 1000;

                }
                OrderTable.AcceptChanges();
                var TotalPrice = Convert.ToDouble(Row["UnitSellingPrice"]) * Convert.ToDouble(Row["LineQuantity"]);
                var NetWeight = Math.Round(Convert.ToDouble(Row["Weight"]) * Convert.ToDouble(Row["LineQuantity"]),0);
                double SellingPrice = Math.Round(Convert.ToDouble(Row["UnitSellingPrice"]),2);
                double LineQuantity = Math.Round(Convert.ToDouble(Row["LineQuantity"]), 2);
                var GrossWeight = NetWeight + 20;
                if (NetWeight > 0)
                {
                    ItemTable.Rows.Add(Row["ItemCode"], Row["ItemDescription"], LineQuantity, Row["SellingUnitDescription"], "3920102899", NetWeight, GrossWeight, SellingPrice, Math.Round(TotalPrice, 2));
                }
                else
                {
                    ItemTable.Rows.Add(Row["ItemCode"], Row["ItemDescription"], LineQuantity, Row["SellingUnitDescription"], "3920102899", "Please Enter", "Please Enter", SellingPrice, Math.Round(TotalPrice, 2));
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
                //general info for order (codes etc)
                foreach (DataRow Row in InvoiceAddTable.Rows)
                {
                    
                    String PUKOrderNum = Row["DocumentNo"].ToString();
                    OrderNumberTextBlock.Text = PUKOrderNum;
                    String CustomerPONum = Row["CustomerDocumentNo"].ToString();
                    CusPOTextBlock.Text = CustomerPONum;
                    String CusTerms = Row["TradingTerms"].ToString();
                    TermsTextBlock.Text = CusTerms;
                    INCOTERMSTextBlock.Document.Blocks.Clear();
                    INCOTERMSTextBlock.AppendText("DAP");
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
                    String ContactEmail = Row["DefaultEmail"].ToString();
                    ContactEmailTextBlock.Document.Blocks.Clear();
                    ContactEmailTextBlock.AppendText(ContactEmail);
                }

                //Pulls just first line for address and delivery to avoid duplication
                if (InvoiceAddTable.Rows.Count > 0)
                {

                    DataRow Row = InvoiceAddTable.Rows[0];

                    String VATNum = Row["TaxRegistrationNumber"].ToString();
                    String CountryCode = Row["Code"].ToString();
                    if (VATNum.Length < 1)
                    {
                        InvToText.AppendText("PLEASE ENTER VAT" + "\r");
                    }
                    else if (CountryCode == "GB" && VATNum.Substring(0, 3) == "GB ")
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
                }

                

            }

        }

        public void ReadWriteCINumber()
        {
            var CurrentUser = Environment.UserName;
            var Filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\drafts";

            string[] fullfiles = Directory.GetFiles(Filepath);
            List<int> filelist = new List<int>();
            
            if (fullfiles.Length > 0)
            {
                foreach (string file in fullfiles)
                {
                    filelist.Add(Convert.ToInt32(System.IO.Path.GetFileName(file).Substring(0, 8)));
                }
            }
            else
            {
                filelist.Add(473);
            }
            int NewCInumber = filelist.Max() + 1;
            var LeadZero = NewCInumber.ToString().Length;
            var ZeroCount = 8 - LeadZero;
            String LeadZeroString = "";
            for (int i = ZeroCount; i > 0; i--)
            {
                LeadZeroString += "0";
            }
            InvNumber.AppendText(LeadZeroString + NewCInumber.ToString());
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
                    AppTheme.ChangeTheme(new Uri("Theme/AppLight.xaml", UriKind.Relative));
                }
                if (themeSetting == "Dark")
                {
                    AppTheme.ChangeTheme(new Uri("Theme/AppDark.xaml", UriKind.Relative));
                }
            }
            return;

        }

        private void SaveDraft()
        {
            var CurrentUser = Environment.UserName;
            var Filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\drafts";

            //Collect variable to save
            var InvNumberRange = new TextRange(InvNumber.Document.ContentStart, InvNumber.Document.ContentEnd);
            String InvNumberTOSAVE = InvNumberRange.Text.Replace("\r", "").Replace("\n", "");
            String OriginTOSAVE = OriginLOC.Text;
            String OrderNumberTOSAVE = OrderNumberTextBlock.Text;
            String CusPOTOSAVE = CusPOTextBlock.Text;
            String TermsTOSAVE = TermsTextBlock.Text;
            var INCOTERMSRange = new TextRange(INCOTERMSTextBlock.Document.ContentStart, INCOTERMSTextBlock.Document.ContentEnd);
            String INCOTERMSTOSAVE = INCOTERMSRange.Text.Replace("\r", "").Replace("\n", "");
            var ContactEmailRange = new TextRange(ContactEmailTextBlock.Document.ContentStart, ContactEmailTextBlock.Document.ContentEnd);
            String ContactEmailTOSAVE = ContactEmailRange.Text.Replace("\r", "").Replace("\n", "");
            var PalletsRange = new TextRange(PalletsTextBlock.Document.ContentStart, PalletsTextBlock.Document.ContentEnd);
            String PalletsTOSAVE = PalletsRange.Text.Replace("\r", "").Replace("\n", "");
            String SubTotalTOSAVE = SubTotTextBlock.Text;
            String VATTOSAVE = VATTextBlock.Text;
            String TotalTOSAVE = TotTextBlock.Text;
            var InvAddressRange = new TextRange(InvToText.Document.ContentStart, InvToText.Document.ContentEnd);
            String InvAddressTOSAVE = InvAddressRange.Text;
            var DelAddressRange = new TextRange(DelToText.Document.ContentStart, DelToText.Document.ContentEnd);
            String DelAddressTOSAVE = DelAddressRange.Text;
            String CertTOSAVE = CertTextBlock.Text;
            String CurrencyTOSAVE = CurrencyTextBlock.Text;

            DataTable ItemTable = new DataTable();
            ItemTable = ((DataView)DataGridCI.ItemsSource).ToTable();

            DataRow dr = ItemTable.Rows[0];

            String CodeTOSAVE = dr["code"].ToString();
            String DescTOSAVE = dr["description"].ToString();
            String QtyTOSAVE = dr["Qty"].ToString();
            String UnitTOSAVE = dr["Unit"].ToString();
            String HSCodeTOSAVE = dr["HSCode"].ToString();
            String WeightTOSave = dr["Weight"].ToString();
            String GrossKGTOSAVE = dr["Grosskg"].ToString();
            String PriceTOSAVE = dr["Price"].ToString();
            String TotalItemTOSAVE = dr["Total"].ToString();

            String ItemLineTwo;
            try
            {
                DataRow dr2 = ItemTable.Rows[1];
                String CodeTOSAVE2 = dr2["code"].ToString();
                String DescTOSAVE2 = dr2["description"].ToString();
                String QtyTOSAVE2 = dr2["Qty"].ToString();
                String UnitTOSAVE2 = dr2["Unit"].ToString();
                String HSCodeTOSAVE2 = dr2["HSCode"].ToString();
                String WeightTOSave2 = dr2["Weight"].ToString();
                String GrossKGTOSAVE2 = dr2["Grosskg"].ToString();
                String PriceTOSAVE2 = dr2["Price"].ToString();
                String TotalItemTOSAVE2 = dr2["Total"].ToString();

                ItemLineTwo = CodeTOSAVE2 + "¬" + DescTOSAVE2 + "¬" + QtyTOSAVE2 + "¬" + UnitTOSAVE2 + "¬" + HSCodeTOSAVE2 + "¬" + WeightTOSave2 + "¬" + GrossKGTOSAVE2 + "¬" + PriceTOSAVE2 + "¬" + TotalItemTOSAVE2;
            }
            catch
            {
                ItemLineTwo = "";
            }
            String ItemLineThree;
            try
            {
                DataRow dr3 = ItemTable.Rows[2];
                String CodeTOSAVE3 = dr3["code"].ToString();
                String DescTOSAVE3 = dr3["description"].ToString();
                String QtyTOSAVE3 = dr3["Qty"].ToString();
                String UnitTOSAVE3 = dr3["Unit"].ToString();
                String HSCodeTOSAVE3 = dr3["HSCode"].ToString();
                String WeightTOSave3 = dr3["Weight"].ToString();
                String GrossKGTOSAVE3 = dr3["Grosskg"].ToString();
                String PriceTOSAVE3 = dr3["Price"].ToString();
                String TotalItemTOSAVE3 = dr3["Total"].ToString();

                ItemLineThree = CodeTOSAVE3 + "¬" + DescTOSAVE3 + "¬" + QtyTOSAVE3 + "¬" + UnitTOSAVE3 + "¬" + HSCodeTOSAVE3 + "¬" + WeightTOSave3 + "¬" + GrossKGTOSAVE3 + "¬" + PriceTOSAVE3 + "¬" + TotalItemTOSAVE3;
            }
            catch
            {
                ItemLineThree = "";
            }
            String ItemLineFour;
            try
            {
                DataRow dr4 = ItemTable.Rows[3];
                String CodeTOSAVE4 = dr4["code"].ToString();
                String DescTOSAVE4 = dr4["description"].ToString();
                String QtyTOSAVE4 = dr4["Qty"].ToString();
                String UnitTOSAVE4 = dr4["Unit"].ToString();
                String HSCodeTOSAVE4 = dr4["HSCode"].ToString();
                String WeightTOSave4 = dr4["Weight"].ToString();
                String GrossKGTOSAVE4 = dr4["Grosskg"].ToString();
                String PriceTOSAVE4 = dr4["Price"].ToString();
                String TotalItemTOSAVE4 = dr4["Total"].ToString();

                ItemLineFour = CodeTOSAVE4 + "¬" + DescTOSAVE4 + "¬" + QtyTOSAVE4 + "¬" + UnitTOSAVE4 + "¬" + HSCodeTOSAVE4 + "¬" + WeightTOSave4 + "¬" + GrossKGTOSAVE4 + "¬" + PriceTOSAVE4 + "¬" + TotalItemTOSAVE4;
            }
            catch
            {
                ItemLineFour = "";
            }

            String ItemLineOne = CodeTOSAVE + "¬" + DescTOSAVE + "¬" + QtyTOSAVE + "¬" + UnitTOSAVE + "¬" + HSCodeTOSAVE + "¬" + WeightTOSave + "¬" + GrossKGTOSAVE + "¬" + PriceTOSAVE + "¬" + TotalItemTOSAVE;
            
            //Create string for each variable on each line
            string[] lines = { InvNumberTOSAVE, OriginTOSAVE, OrderNumberTOSAVE, CusPOTOSAVE, TermsTOSAVE, INCOTERMSTOSAVE, ContactEmailTOSAVE, PalletsTOSAVE, SubTotalTOSAVE, VATTOSAVE, TotalTOSAVE, "---", ItemLineOne, "---", ItemLineTwo, "---", ItemLineThree, "---", ItemLineFour, InvAddressTOSAVE, "****", DelAddressTOSAVE, "****", CertTOSAVE, CurrencyTOSAVE };

            string[] FilesStringArray = Directory.GetFiles(Filepath);
            foreach (string FileName in FilesStringArray)
            {
                if (FileName.Contains(OrderNumberTOSAVE) && !FileName.Contains(InvNumberTOSAVE))
                {
                    var DiagBox = MessageBox.Show("This will replace the old draft", "Old Draft", MessageBoxButton.OKCancel);
                    if (DiagBox == MessageBoxResult.OK)
                    {
                        File.Delete(FileName);
                    }
                    else
                    {
                        return;
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(Filepath, InvNumberTOSAVE + " - " + OrderNumberTOSAVE + ".txt")))
            {
                foreach (string line in lines)
                    sw.WriteLine(line);
            }
            MessageBox.Show("Saved Draft!");
        }

        private void DetailsRecall()
        {
            var CurrentUser = Environment.UserName;
            var Folderpath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\drafts";
            var OrderNum = TxtBxSearch.Text;

            string[] FilesStringArray = Directory.GetFiles(Folderpath);
            String AllFiles = String.Concat(FilesStringArray);

            if(AllFiles.Contains(OrderNum))
            {
                foreach (String SingleString in FilesStringArray)
                {
                    if (SingleString.Contains(OrderNum))
                    {
                        var filename = SingleString.ToString();
                        var DiagResult = MessageBox.Show("A draft already exists for this order, recall?", filename, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                        if (DiagResult == MessageBoxResult.Yes)
                        {
                            PUKLogo.Visibility = Visibility.Visible;
                            GeneratedBorder.Visibility = Visibility.Visible;
                            BtnSaveCI.Visibility = Visibility.Visible;
                            SearchBorder.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                            SearchBorder.Width = 220;
                            SearchBorder.CornerRadius = new CornerRadius(10, 0, 0, 10);
                            SearchTextBoxBackground.Visibility = Visibility.Hidden;
                            TxtBxSearch.Visibility = Visibility.Hidden;
                            BtnGenCI.Visibility = Visibility.Hidden;
                            OrderNumText.Visibility = Visibility.Hidden;

                            var converter = new System.Windows.Media.BrushConverter();
                            var brush = (System.Windows.Media.Brush)converter.ConvertFromString("#FF0000");


                            DataTable RecallTable = new DataTable();
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


                            string[] lines = File.ReadAllLines(SingleString);
                            var linecount = lines.Length;
                            int LineDiff = 37 - linecount;

                            string[] linesaddress = lines.Skip(19).ToArray();
                            int counter = 0;

                            RecallTable.Columns.Clear();
                            var currentdate = (DateTime.Now).ToString().Substring(0, 10);
                            for (int col=0; col<linecount; col++)
                                RecallTable.Columns.Add(new DataColumn("Column" + (col+1).ToString()));

                            RecallTable.Rows.Add(lines[0], lines[1], lines[2], lines[3], lines[4], lines[5], lines[6], lines[7], lines[8], lines[9], lines[10]);

                            string[] itemlines = { lines[12], lines[14], lines[16], lines[18] };
                            foreach (string itemline in itemlines)
                            {
                                if (itemline.Length > 0)
                                {
                                    string[] items = itemline.Split("¬");
                                    ItemTable.Rows.Add(items);
                                }
                            }
                            DataGridCI.ItemsSource = ItemTable.DefaultView;

                            DataRow Row = RecallTable.Rows[0];

                            InvFromText.AppendText("XI903824828000" + "\r" + "Polythene UK Ltd" + "\r" + "31c Avenue 1" + "\r" + "Station Lane" + "\r" + "Witney" + "\r" + "OX28 4XZ" + "\r" + "0845 643 1601");

                            InvNumber.Document.Blocks.Clear();
                            InvNumber.AppendText(Row["Column1"].ToString());
                            InvDate.Text = currentdate;
                            OriginLOC.Text = Row["Column2"].ToString();
                            OrderNumberTextBlock.Text = Row["Column3"].ToString();
                            CusPOTextBlock.Text = Row["Column4"].ToString();
                            TermsTextBlock.Text = Row["Column5"].ToString();
                            ContactEmailTextBlock.Document.Blocks.Clear();
                            ContactEmailTextBlock.AppendText(Row["Column7"].ToString()); 
                            INCOTERMSTextBlock.Document.Blocks.Clear();
                            INCOTERMSTextBlock.AppendText(Row["Column6"].ToString());
                            PalletsTextBlock.Document.Blocks.Clear();
                            PalletsTextBlock.AppendText(Row["Column8"].ToString());
                            SubTotTextBlock.Text = Row["Column9"].ToString();
                            VATTextBlock.Text = Row["Column10"].ToString();
                            TotTextBlock.Text = Row["Column11"].ToString();

                            CertTextBlock.Text = lines[35 - LineDiff].ToString();
                            CurrencyTextBlock.Text = lines[36 - LineDiff].ToString();
                            

                            InvToText.Document.Blocks.Clear();
                            DelToText.Document.Blocks.Clear();

                            foreach (string linetest in linesaddress)
                            {
                                if (linetest.Length > 0 && linetest is not "****")
                                {
                                    counter++;
                                    InvToText.AppendText(linetest + "\r");
                                }
                                else if (linetest is "****")
                                {

                                    int FirstSplit = counter;
                                    string[] linesaddresssplit = linesaddress.Skip(FirstSplit + 3).ToArray();

                                    foreach(string lineaddresstest in linesaddresssplit)
                                    {
                                        if (lineaddresstest.Length > 0 && lineaddresstest is not "****")
                                        {
                                            DelToText.AppendText(lineaddresstest + "\r");
                                        }
                                        else if (lineaddresstest is "****")
                                        {
                                            MessageBox.Show("Loaded Successfully, please check all information is correct and filled in");
                                            return;
                                        }
                                    }
                                }
                            }

                        }

                        else if(DiagResult == MessageBoxResult.No)
                        {
                            PUKLogo.Visibility = Visibility.Visible;
                            GeneratedBorder.Visibility = Visibility.Visible;
                            BtnSaveCI.Visibility = Visibility.Visible;
                            SearchBorder.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                            SearchBorder.Width = 220;
                            SearchBorder.CornerRadius = new CornerRadius(10, 0, 0, 10);
                            SearchTextBoxBackground.Visibility = Visibility.Hidden;
                            TxtBxSearch.Visibility = Visibility.Hidden;
                            BtnGenCI.Visibility = Visibility.Hidden;
                            OrderNumText.Visibility = Visibility.Hidden;

                            var converter = new System.Windows.Media.BrushConverter();
                            var brush = (System.Windows.Media.Brush)converter.ConvertFromString("#FF0000");

                            var currentdate = (DateTime.Now).ToString().Substring(0, 10);

                            
                            OrderDataSQL();
                            DetailsSQL();
                            ReadWriteCINumber();
                            InvFromText.AppendText("XI903824828000" + "\r" + "Polythene UK Ltd" + "\r" + "31c Avenue 1" + "\r" + "Station Lane" + "\r" + "Witney" + "\r" + "OX28 4XZ" + "\r" + "0845 643 1601");
                            InvDate.Text = currentdate;

                            MessageBox.Show("Please check all information is correct and filled in");
                        }
                        else if (DiagResult == MessageBoxResult.Cancel) 
                        {
                        
                        }
                    }

                }
            }
            else
            {
                PUKLogo.Visibility = Visibility.Visible;
                GeneratedBorder.Visibility = Visibility.Visible;
                BtnSaveCI.Visibility = Visibility.Visible;
                SearchBorder.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                SearchBorder.Width = 220;
                SearchBorder.CornerRadius = new CornerRadius(10, 0, 0, 10);
                SearchTextBoxBackground.Visibility = Visibility.Hidden;
                TxtBxSearch.Visibility = Visibility.Hidden;
                BtnGenCI.Visibility = Visibility.Hidden;
                OrderNumText.Visibility = Visibility.Hidden;

                var converter = new System.Windows.Media.BrushConverter();
                var brush = (System.Windows.Media.Brush)converter.ConvertFromString("#FF0000");

                var currentdate = (DateTime.Now).ToString().Substring(0, 10);

                InvFromText.AppendText("XI903824828000" + "\r" + "Polythene UK Ltd" + "\r" + "31c Avenue 1" + "\r" + "Station Lane" + "\r" + "Witney" + "\r" + "OX28 4XZ" + "\r" + "0845 643 1601");
                OriginLOC.Text = "UK";
                InvDate.Text = currentdate;

                OrderDataSQL();
                DetailsSQL();
                ReadWriteCINumber();

                MessageBox.Show("Please check all information is correct and filled in");
            }
        }

        private void BtnSaveCI_Click(object sender, RoutedEventArgs e)
        {
            SaveDraft(); 

        }


    }
}
