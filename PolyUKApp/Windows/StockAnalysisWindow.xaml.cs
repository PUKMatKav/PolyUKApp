using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using Microsoft.Data.SqlClient;
using PolyUKApp.SQL;
using PolyUKApp.SQL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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
    public class ViewModel
    {        
        public ISeries[] Series { get; set; } = new ISeries[]
        {
             
                new LineSeries<int>
                {
                    Values = new int[] { 4, 6, 5, 3, -3, -1, 2 }
                }
        };
    }

    /// <summary>
    /// Interaction logic for StockAnalysisWindow.xaml
    /// </summary>
    public partial class StockAnalysisWindow : Window
    {
        List<Item> ItemCode = new List<Item>();
        List<string> CodeCheck = new List<string>();
        String connectionstring = DataAccess.GlobalSQL.Connection;
        String CurrentUser = Environment.UserName;
        String currentCode = "";
        String closestCode = "";
        int currentLowestCompute = 99;

        public StockAnalysisWindow()
        {
            InitializeComponent();
            LoadTheme();
            ItemCodeList();
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

        private void TopBar0_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            currentCode = SearchTextBox.Text;

            foreach (string str in CodeCheck)
            {
                int currentLevenshtein = LevenshteinDistance.Compute(currentCode, str);
            }

            LoadItemInfo();
            LoadBatchInfo();
            LoadAllocationInfo();
            BatchGrid.ItemsSource = null;
            BtnCheckALLPrice.Visibility = Visibility.Collapsed;
            CheckMessageBlock.Visibility = Visibility.Collapsed;
            BtnCheckItemPrice.Visibility = Visibility.Visible;
        }

        private void LoadItemInfo()
        {
            RichTextDesc.Document.Blocks.Clear();
            RichTextSpec.Document.Blocks.Clear();
            RichTextFreeStock.Document.Blocks.Clear();
            RichTextUnit.Document.Blocks.Clear();
            RichTextWeight.Document.Blocks.Clear();
            RichTextAvPrice.Document.Blocks.Clear();
            RichTextTrendPrice.Document.Blocks.Clear();


            DataTable ItemTable = new DataTable();

            using (SqlConnection _con = new SqlConnection(connectionstring))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.ItemAnalysisQuery;

                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    _cmd.Parameters.AddWithValue("@Code", currentCode);
                    _dap.Fill(ItemTable);
                }
            }

            double avPrice = Convert.ToDouble(ItemTable.Rows[0]["AverageBuyingPrice"]);
            double lastPrice = Convert.ToDouble(ItemTable.Rows[0]["CostPrice"]);
            double trendPrice = Math.Round((lastPrice - avPrice) / avPrice * 100, 2);
            double itemWeight = Math.Round(Convert.ToDouble(ItemTable.Rows[0]["Weight"]), 2);
            decimal itemQty = Convert.ToDecimal(ItemTable.Rows[0]["FreeStockQuantity"]);
            string ItemQtyString = itemQty.ToString("G29");

            RichTextDesc.AppendText(ItemTable.Rows[0]["Name"].ToString());
            RichTextSpec.AppendText(ItemTable.Rows[0]["Description"].ToString());
            RichTextFreeStock.AppendText(ItemQtyString);
            RichTextUnit.AppendText(ItemTable.Rows[0]["StockUnitName"].ToString());
            RichTextWeight.AppendText(itemWeight + "kg");
            RichTextAvPrice.AppendText("£" + avPrice.ToString());
            RichTextTrendPrice.AppendText("£" + lastPrice + " (" + trendPrice.ToString() + "%)");
            
        }

        private void LoadBatchInfo()
        {
            DataTable BatchTable = new DataTable();

            using (SqlConnection _con = new SqlConnection(connectionstring))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.ItemBatchQuery;
                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap  =new SqlDataAdapter(_cmd);
                    _cmd.Parameters.AddWithValue("@Code", currentCode);
                    _dap.Fill(BatchTable);
                }
            }
            BatchTable.Columns.Add("FreeStock");

            foreach (DataRow row in BatchTable.Rows)
            {
                row["FreeStock"] = Convert.ToDouble(row["GoodsInQuantity"]) - Convert.ToDouble(row["GoodsOutQuantity"]) - Convert.ToDouble(row["AllocatedQuantity"]);
            }
            BatchTable.AcceptChanges();

            foreach (DataRow row in BatchTable.Rows)
            {
                if (row["FreeStock"] is not "0")
                {
                }
                else
                {
                    row.Delete();
                }
            }
            BatchTable.Columns.Remove("TraceableItemID");
            BatchTable.Columns.Remove("ReceiptDate");
            BatchTable.Columns.Remove("Code");
            BatchTable.Columns.Remove("GoodsInQuantity");
            BatchTable.Columns.Remove("GoodsOutQuantity");
            BatchTable.Columns.Remove("AllocatedQuantity");
            BatchTable.Columns["FreeStock"].SetOrdinal(1);
            BatchTable.Columns["WarehouseName"].SetOrdinal(2);
            BatchTable.AcceptChanges();

            BatchGrid.ItemsSource = BatchTable.DefaultView;
            //System.Windows.MessageBox.Show("Done");
        }

        private void LoadAllocationInfo()
        {
            DataTable AllocationTable = new DataTable();

            using (SqlConnection _con = new SqlConnection(connectionstring))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.ItemAllocatedBatchQuery;
                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    _cmd.Parameters.AddWithValue("@Code", currentCode);
                    _dap.Fill(AllocationTable);

                }
            }
            foreach (DataRow row in AllocationTable.Rows)
            {
                decimal decQty = Convert.ToDecimal(row["AllocatedQuantity"]);
                string newQty = decQty.ToString("G29");
                row["AllocatedQuantity"] = newQty;
            }
            AllocationTable.Columns["IdentificationNo"].SetOrdinal(0);
            AllocationTable.Columns["AllocatedQuantity"].SetOrdinal(1);
            AllocationTable.Columns["AllocatedQuantity"].ColumnName = "FreeStock";
            AllocationTable.Columns["RecipientName"].ColumnName = "RecipientNameIsLonger";
            AllocationTable.AcceptChanges();
            AllocatedBatchGrid.ItemsSource = AllocationTable.DefaultView;
            //System.Windows.MessageBox.Show("Done");
        }

        private void CostPriceCheckerALL()
        {
            String filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Admin\\Admin Stock NEW.xlsx";

            DataTable AdminSheetTable = new DataTable("AdminSheetTable");
            DataTable SupplierOfficeTable = new DataTable();
            OleDbConnection oleExcelConnection = default(OleDbConnection);

            var Connection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
            oleExcelConnection = new OleDbConnection(Connection);

            using (OleDbCommand _cmd = new OleDbCommand())
            {
                _cmd.Connection = oleExcelConnection;
                _cmd.CommandText = "SELECT Batch, [Product Code], [PO Cost] FROM [HACKLINGS STOCK$] " +
                    "WHERE Batch IS NOT NULL " +
                    "UNION ALL " +
                    "SELECT Batch, [Product Code], [PO Cost] FROM [SUPPLIER STOCK$] " +
                    "WHERE Batch IS NOT NULL " +
                    "UNION ALL " +
                    "SELECT Batch, [Product Code], [PO Cost] FROM [OFFICE$] " +
                    "WHERE Batch IS NOT NULL";

                using (OleDbDataAdapter _dap = new OleDbDataAdapter())
                {
                    _dap.SelectCommand = _cmd;
                    _dap.Fill(AdminSheetTable);

                    //System.Windows.MessageBox.Show("Done");
                }
            }

            List<string> AdminSheetBatchList = new List<string>();
            foreach (DataRow row in AdminSheetTable.Rows)
            {
                if (AdminSheetBatchList.Contains(row["Batch"].ToString()))
                {
                    row.Delete();
                }
                else
                {
                    AdminSheetBatchList.Add(row["Batch"].ToString());
                }
            }
            AdminSheetTable.AcceptChanges();

            DataTable SageItemTable = new DataTable("SageItemTable");

            using (SqlConnection _con = new SqlConnection(connectionstring))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.ItemAnalysisQueryALL;

                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    _dap.Fill(SageItemTable);

                }

                List<string> BatchList = new List<string>();

                foreach (DataRow row in SageItemTable.Rows)
                {
                    if (row["MovementReference"].ToString().Length == 10 && row["MovementReference"].ToString().Substring(0,4) == "0000")
                    {
                        row["MovementReference"] = row["MovementReference"].ToString().Substring(4, 6);
                        if (BatchList.Contains(row["MovementReference"].ToString()))
                        {
                            row.Delete();
                        }
                        else
                        {
                            BatchList.Add(row["MovementReference"].ToString());
                        }
                    }
                    else
                    {
                        if (BatchList.Contains(row["MovementReference"].ToString()))
                        {
                            row.Delete();
                        }
                        else
                        {
                            BatchList.Add(row["MovementReference"].ToString());
                        }
                    }
                }
            }
            SageItemTable.Columns.Remove("ItemID");
            SageItemTable.Columns.Remove("DateTimeCreated");
            SageItemTable.Columns.Remove("OpeningStockLevel");
            SageItemTable.Columns.Remove("Name");
            SageItemTable.Columns.Remove("Description");
            SageItemTable.Columns.Remove("FreeStockQuantity");
            SageItemTable.Columns.Remove("StockUnitName");
            SageItemTable.Columns.Remove("AverageBuyingPrice");
            SageItemTable.Columns.Remove("Weight");
            SageItemTable.Columns["MovementReference"].SetOrdinal(0);
            AdminSheetTable.Columns["Batch"].ColumnName = "MovementReference";
            SageItemTable.AcceptChanges();
            AdminSheetTable.AcceptChanges();


            SageItemTable.PrimaryKey = new DataColumn[] { SageItemTable.Columns["MovementReference"] };
            AdminSheetTable.PrimaryKey = new DataColumn[] { AdminSheetTable.Columns["MovementReference"] };
            AdminSheetTable.Merge(SageItemTable);

            AdminSheetTable.Columns.Remove("Code");
            AdminSheetTable.Columns.Remove("Product Code");
            AdminSheetTable.Columns["MovementReference"].ColumnName = "Batch Longer";
            AdminSheetTable.Columns["CostPrice"].SetOrdinal(1);
            AdminSheetTable.Columns["CostPrice"].ColumnName = "Sage Price Long";
            AdminSheetTable.Columns["PO Cost"].ColumnName = "Sheet Price Long";
            AdminSheetTable.Columns.Add("Notes");
            AdminSheetTable.AcceptChanges();

            foreach (DataRow row in AdminSheetTable.Rows)
            {
                if (row["Sage Price Long"] == DBNull.Value && row["Sheet Price Long"] != DBNull.Value)
                {
                    row.Delete();
                }
                else if (row["Sage Price Long"] != DBNull.Value && row["Sheet Price Long"] == DBNull.Value)
                {
                    row.Delete();
                }
                else if (row["Sage Price Long"] == DBNull.Value && row["Sheet Price Long"] == DBNull.Value)
                {
                    row.Delete();
                }
                else
                {
                    var Sage2Digits = Math.Round(Convert.ToDouble(row["Sage Price Long"]), 2);
                    if (Sage2Digits > Math.Round(Convert.ToDouble(row["Sheet Price Long"]), 2))
                    {
                        row["Sage Price Long"] = Sage2Digits;
                        row["Notes"] = "Lower on Sheet";
                    }
                    else if (Sage2Digits < Math.Round(Convert.ToDouble(row["Sheet Price Long"]), 2))
                    {
                        row["Sage Price Long"] = Sage2Digits;
                        row["Notes"] = "Higher on Sheet";
                    }
                    else
                    {
                        row.Delete();
                    }
                }

            }
            AdminSheetTable.AcceptChanges();

            CostPriceBatchGrid.ItemsSource = AdminSheetTable.DefaultView;
            if (AdminSheetTable.Rows.Count == 0)
            {
                CheckMessageBlock.Visibility = Visibility.Visible;
            }

            System.Windows.MessageBox.Show("Done");

        }

        private void CostPriceCheckerITEM()
        {
            String filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Admin\\Admin Stock NEW.xlsx";

            DataTable AdminSheetTable = new DataTable("AdminSheetTable");
            DataTable SupplierOfficeTable = new DataTable();
            OleDbConnection oleExcelConnection = default(OleDbConnection);

            var Connection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
            oleExcelConnection = new OleDbConnection(Connection);

            using (OleDbCommand _cmd = new OleDbCommand())
            {
                _cmd.Connection = oleExcelConnection;
                _cmd.Parameters.AddWithValue("@Code", SearchTextBox.Text);
                _cmd.CommandText = "SELECT Batch, [Product Code], [PO Cost] FROM [HACKLINGS STOCK$] " +
                    "WHERE [Product Code] = @Code AND Batch IS NOT NULL " +
                    "UNION ALL " +
                    "SELECT Batch, [Product Code], [PO Cost] FROM [SUPPLIER STOCK$] " +
                    "WHERE [Product Code] = @Code AND Batch IS NOT NULL " +
                    "UNION ALL " +
                    "SELECT Batch, [Product Code], [PO Cost] FROM [OFFICE$] " +
                    "WHERE [Product Code] = @Code AND Batch IS NOT NULL";

                using (OleDbDataAdapter _dap = new OleDbDataAdapter())
                {
                    _dap.SelectCommand = _cmd;
                    _dap.Fill(AdminSheetTable);

                    //System.Windows.MessageBox.Show("Done");
                }
            }

            List<string> AdminSheetBatchList = new List<string>();
            foreach (DataRow row in AdminSheetTable.Rows)
            {
                if (AdminSheetBatchList.Contains(row["Batch"].ToString()))
                {
                    row.Delete();
                }
                else
                {
                    AdminSheetBatchList.Add(row["Batch"].ToString());
                }
            }
            AdminSheetTable.AcceptChanges();

            DataTable SageItemTable = new DataTable("SageItemTable");

            using (SqlConnection _con = new SqlConnection(connectionstring))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.ItemAnalysisQuery;

                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    _cmd.Parameters.AddWithValue("@Code", SearchTextBox.Text);
                    _dap.Fill(SageItemTable);

                }

                List<string> BatchList = new List<string>();

                foreach (DataRow row in SageItemTable.Rows)
                {
                    if (row["MovementReference"].ToString().Length == 10 && row["MovementReference"].ToString().Substring(0, 4) == "0000")
                    {
                        row["MovementReference"] = row["MovementReference"].ToString().Substring(4, 6);
                        if (BatchList.Contains(row["MovementReference"].ToString()))
                        {
                            row.Delete();
                        }
                        else
                        {
                            BatchList.Add(row["MovementReference"].ToString());
                        }
                    }
                    else
                    {
                        if (BatchList.Contains(row["MovementReference"].ToString()))
                        {
                            row.Delete();
                        }
                        else
                        {
                            BatchList.Add(row["MovementReference"].ToString());
                        }
                    }
                }
            }
            SageItemTable.Columns.Remove("ItemID");
            SageItemTable.Columns.Remove("DateTimeCreated");
            SageItemTable.Columns.Remove("OpeningStockLevel");
            SageItemTable.Columns.Remove("Name");
            SageItemTable.Columns.Remove("Description");
            SageItemTable.Columns.Remove("FreeStockQuantity");
            SageItemTable.Columns.Remove("StockUnitName");
            SageItemTable.Columns.Remove("AverageBuyingPrice");
            SageItemTable.Columns.Remove("Weight");
            SageItemTable.Columns["MovementReference"].SetOrdinal(0);
            AdminSheetTable.Columns["Batch"].ColumnName = "MovementReference";
            SageItemTable.AcceptChanges();
            AdminSheetTable.AcceptChanges();


            SageItemTable.PrimaryKey = new DataColumn[] { SageItemTable.Columns["MovementReference"] };
            AdminSheetTable.PrimaryKey = new DataColumn[] { AdminSheetTable.Columns["MovementReference"] };
            AdminSheetTable.Merge(SageItemTable);

            AdminSheetTable.Columns.Remove("Code");
            AdminSheetTable.Columns.Remove("Product Code");
            AdminSheetTable.Columns["MovementReference"].ColumnName = "Batch Longer";
            AdminSheetTable.Columns["CostPrice"].SetOrdinal(1);
            AdminSheetTable.Columns["CostPrice"].ColumnName = "Sage Price Long";
            AdminSheetTable.Columns["PO Cost"].ColumnName = "Sheet Price Long";
            AdminSheetTable.Columns.Add("Notes");
            AdminSheetTable.AcceptChanges();

            foreach (DataRow row in AdminSheetTable.Rows)
            {
                if (row["Sage Price Long"] == DBNull.Value && row["Sheet Price Long"] != DBNull.Value)
                {
                    row.Delete();
                }
                else if (row["Sage Price Long"] != DBNull.Value && row["Sheet Price Long"] == DBNull.Value)
                {
                    row.Delete();
                }
                else if (row["Sage Price Long"] == DBNull.Value && row["Sheet Price Long"] == DBNull.Value)
                {
                    row.Delete();
                }
                else
                {
                    var Sage2Digits = Math.Round(Convert.ToDouble(row["Sage Price Long"]), 2);
                    if (Sage2Digits > Math.Round(Convert.ToDouble(row["Sheet Price Long"]), 2))
                    {
                        row["Sage Price Long"] = Sage2Digits;
                        row["Notes"] = "Lower on Sheet";
                    }
                    else if (Sage2Digits < Math.Round(Convert.ToDouble(row["Sheet Price Long"]), 2))
                    {
                        row["Sage Price Long"] = Sage2Digits;
                        row["Notes"] = "Higher on Sheet";
                    }
                    else
                    {
                        row.Delete();
                    }
                }

            }
            AdminSheetTable.AcceptChanges();

            CostPriceBatchGrid.ItemsSource = AdminSheetTable.DefaultView;
            if (AdminSheetTable.Rows.Count == 0)
            {
                CheckMessageBlock.Visibility = Visibility.Visible;
            }

            System.Windows.MessageBox.Show("Done");

        }

        private void BtnCheckALLPrice_Click(object sender, RoutedEventArgs e)
        {
            CostPriceCheckerALL();
        }

        private void BtnCheckItemPrice_Click(object sender, RoutedEventArgs e)
        {
            CostPriceCheckerITEM();
        }

        static class LevenshteinDistance
        {
            /// approximate string matching
            public static int Compute(string s, string t)
            {
                int n = s.Length;
                int m = t.Length;
                int[,] d = new int[n + 1, m + 1];

                //step 1
                if (n == 0)
                {
                    return m;
                }
                if (m == 0)
                {
                    return n;
                }
                //step 2
                for (int i = 0; i <= n; d[i, 0] = i++)
                {

                }
                for (int j = 0; j <= m; d[0, j] = j++)
                {

                }
                //step 3
                for (int i = 1; i <= n; i++)
                {
                    for (int j = 1; j <= m; j++)
                    {
                        // Step 5
                        int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                        // Step 6
                        d[i, j] = Math.Min(
                            Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                            d[i - 1, j - 1] + cost);
                    }
                }
                //step 7
                return d[n, m];
            }
        }

        private void ItemCodeList()
        {
            DataTable CodeTable = new DataTable();
            using (SqlConnection _con = new SqlConnection(connectionstring))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.StockItemNames;

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    _con.Open();
                    _dap.Fill(CodeTable);
                    CodeCheck = CodeTable.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("Code")).ToList();
                }
            }
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

    }
}
