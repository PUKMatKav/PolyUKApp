using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using LiveCharts;
using LiveCharts.Wpf;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Data.SqlClient;
using Org.BouncyCastle.Utilities;
using PolyUKApp.SQL;
using PolyUKApp.SQL.Models;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Threading;



namespace PolyUKApp.Windows
{

    /// <summary>
    /// Interaction logic for StockAnalysisWindow.xaml
    /// </summary>
    /// 

    public partial class StockAnalysisWindow : Window
    {
        List<string> CodeCheck = new List<string>();
        String connectionstring = DataAccess.GlobalSQL.Connection;
        String CurrentUser = Environment.UserName;
        String currentCode = "";
        String closestCode = "";
        int currentLowestCompute = 99;
        bool codeMatch = false;
        DispatcherTimer timer = new DispatcherTimer();
        bool BtnCheckAllclicked = false;
        bool BtnCheckAllclicked2 = false;
        bool BtnResetclicked = false;
        bool BtnResetclicked2 = false;
        bool BtnSearchClicked = false;
        bool BtnResetSearchClicked = false;

        public SeriesCollection MySeries { get; set; }
        public string[] BarLabels { get; set; }
        public Func<double, string> Formatter { get; set; }


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

            codeMatch = false;

            currentCode = SearchTextBox.Text.ToUpper();
            BtnSearch.Visibility = Visibility.Collapsed;
            BtnSearchReset.Visibility = Visibility.Visible;

            foreach (string str in CodeCheck)
            {
                if (str == currentCode)
                {
                    codeMatch = true;
                }
            }
            if (codeMatch)
            {
                BatchGrid.ItemsSource = null;
                AllocatedBatchGrid.ItemsSource = null;
                BtnSearchClicked = true;
                LoadItemInfo();
                LoadBatchInfo();
                LoadAllocationInfo();
                DispatcherTimer timer = new DispatcherTimer();
                timer.Tick += new EventHandler(SmoothGridMove_Tick);
                timer.Interval = TimeSpan.FromMicroseconds(750);
                timer.Start();
                GraphDataLoad(currentCode);
                BtnCheckALLPrice.Visibility = Visibility.Collapsed;
                CheckMessageBlock.Visibility = Visibility.Collapsed;

            }
            else
            {
                foreach (string str in CodeCheck)
                {
                    int currentLevenshtein = LevenshteinDistance.Compute(currentCode, str);
                    if (currentLevenshtein < currentLowestCompute)
                    {
                        currentLowestCompute = currentLevenshtein;
                        closestCode = str;
                    }
                }
                MessageBoxResult mbr = System.Windows.MessageBox.Show("Did you mean " + closestCode + "?", "Closely Matching Code", MessageBoxButton.YesNo);
                if (mbr == MessageBoxResult.Yes)
                {
                    currentCode = closestCode;
                    BatchGrid.ItemsSource = null;
                    AllocatedBatchGrid.ItemsSource = null;
                    BtnSearchClicked = true;
                    LoadItemInfo();
                    LoadBatchInfo();
                    LoadAllocationInfo();
                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Tick += new EventHandler(SmoothGridMove_Tick);
                    timer.Interval = TimeSpan.FromMicroseconds(750);
                    timer.Start();
                    GraphDataLoad(currentCode);
                    BtnCheckALLPrice.Visibility = Visibility.Collapsed;
                    CheckMessageBlock.Visibility = Visibility.Collapsed;

                }
                else if (mbr == MessageBoxResult.No)
                {
                    System.Windows.MessageBox.Show("Code not found, please try again");
                    BtnSearchReset.Visibility = Visibility.Collapsed;
                    BtnSearch.Visibility = Visibility.Visible;
                    codeMatch = false;
                    currentCode = "";
                    closestCode = "";
                    SearchTextBox.Text = "";
                    currentLowestCompute = 99;
                }
            }
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


            System.Data.DataTable ItemTable = new System.Data.DataTable();

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
            System.Data.DataTable BatchTable = new System.Data.DataTable();

            using (SqlConnection _con = new SqlConnection(connectionstring))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.ItemBatchQuery;
                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    _cmd.Parameters.AddWithValue("@Code", currentCode);
                    _dap.Fill(BatchTable);
                }
            }
            BatchTable.Columns.Add("FreeStock");

            foreach (DataRow row in BatchTable.Rows)
            {
                if (row["WarehouseName"] != DBNull.Value)
                {
                    row["FreeStock"] = Convert.ToDouble(row["GoodsInQuantity"]) - Convert.ToDouble(row["GoodsOutQuantity"]) - Convert.ToDouble(row["AllocatedQuantity"]);
                }
            }
            BatchTable.AcceptChanges();

            foreach (DataRow row in BatchTable.Rows)
            {
                if (row["FreeStock"] == DBNull.Value)
                {
                    row.Delete();
                }
                else if (row["FreeStock"] is not "0")
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
            System.Data.DataTable AllocationTable = new System.Data.DataTable();

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

        /*private void CostPriceCheckerALL()
        {
            System.Windows.MessageBox.Show("Please close Admin Stock Sheet (if open) before continuing");

            String filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Admin\\Admin Stock NEW.xlsx";

            DataTable AdminSheetTable = new DataTable("AdminSheetTable");
            DataTable SupplierOfficeTable = new DataTable();
            OleDbConnection oleExcelConnection = default(OleDbConnection);

            var Connection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
            oleExcelConnection = new OleDbConnection(Connection);

            using (OleDbCommand _cmd = new OleDbCommand())
            {
                _cmd.Connection = oleExcelConnection;
                _cmd.CommandText = "SELECT Batch, [Product Code], [PO Cost], [Quantity], [Warehouse] FROM [HACKLINGS STOCK$] " +
                    "WHERE Batch IS NOT NULL " +
                    "UNION ALL " +
                    "SELECT Batch, [Product Code], [PO Cost], [Quantity], [Warehouse] FROM [SUPPLIER STOCK$] " +
                    "WHERE Batch IS NOT NULL " +
                    "UNION ALL " +
                    "SELECT Batch, [Product Code], [PO Cost], [Quantity], [Warehouse] FROM [OFFICE$] " +
                    "WHERE Batch IS NOT NULL";

                using (OleDbDataAdapter _dap = new OleDbDataAdapter())
                {
                    _dap.SelectCommand = _cmd;
                    _dap.Fill(AdminSheetTable);

                    //System.Windows.MessageBox.Show("Done");
                }
            }
            AdminSheetTable.Columns.Add("ID");
            foreach (DataRow row in AdminSheetTable.Rows)
            {
                if (row["Warehouse"].ToString() == "Hacks" || row["Warehouse"].ToString() == "Hack" || row["Warehouse"].ToString() == "Hacklings")
                {
                    row["ID"] = row["Batch"].ToString() + "Hacklings";
                }
                else if (row["Warehouse"].ToString() == "Office")
                {
                    row["ID"] = row["Batch"].ToString() + "Office - Witney";
                }
                else
                {
                    row["ID"] = row["Batch"].ToString() + row["Warehouse"].ToString();
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

                SageItemTable.Columns.Add("ID");
                SageItemTable.Columns["Name1"].ColumnName = "Location";
                foreach (DataRow row in SageItemTable.Rows)
                {
                    if (row["MovementReference"].ToString().Length == 10 && row["MovementReference"].ToString().Substring(0, 4) == "0000")
                    {
                        row["MovementReference"] = row["MovementReference"].ToString().Substring(4, 6);
                        row["ID"] = row["MovementReference"].ToString() + row["Location"].ToString();
                    }
                    else
                    {
                        row["ID"] = row["MovementReference"].ToString() + row["Location"].ToString();
                    }
                }

                List<string> BatchList = new List<string>();

                foreach (DataRow row in SageItemTable.Rows)
                {
                    if (BatchList.Contains(row["ID"].ToString()))
                    {
                        row.Delete();
                    }
                    else
                    {
                        BatchList.Add(row["ID"].ToString());
                    }
                }
            }
            SageItemTable.Columns.Remove("ItemID");
            SageItemTable.Columns.Remove("DateTimeCreated");

            SageItemTable.Columns.Remove("Name");
            SageItemTable.Columns.Remove("Description");
            SageItemTable.Columns.Remove("FreeStockQuantity");
            SageItemTable.Columns.Remove("StockUnitName");
            SageItemTable.Columns.Remove("AverageBuyingPrice");
            SageItemTable.Columns.Remove("Weight");
            SageItemTable.Columns["MovementReference"].SetOrdinal(0);
            //SageItemTable.Columns["OpeningStockLevel"];
            AdminSheetTable.Columns["Batch"].ColumnName = "MovementReference";
            AdminSheetTable.Columns["Quantity"].ColumnName = "SheetQtyLng";
            SageItemTable.AcceptChanges();
            AdminSheetTable.AcceptChanges();

            DataTable ClonedAdminSheetTable = AdminSheetTable.Clone();
            ClonedAdminSheetTable.Columns[3].DataType = typeof(decimal);
            foreach (DataRow dr in AdminSheetTable.Rows)
            {
                ClonedAdminSheetTable.ImportRow(dr);
            }


            SageItemTable.PrimaryKey = new DataColumn[] { SageItemTable.Columns["ID"] };
            ClonedAdminSheetTable.PrimaryKey = new DataColumn[] { ClonedAdminSheetTable.Columns["ID"] };
            ClonedAdminSheetTable.Merge(SageItemTable);

            ClonedAdminSheetTable.Columns.Remove("Code");
            ClonedAdminSheetTable.Columns.Remove("Product Code");
            ClonedAdminSheetTable.Columns["MovementReference"].ColumnName = "Batch Longer";
            ClonedAdminSheetTable.Columns["CostPrice"].SetOrdinal(1);
            ClonedAdminSheetTable.Columns["OpeningStockLevel"].SetOrdinal(2);
            ClonedAdminSheetTable.Columns["OpeningStockLevel"].ColumnName = "OpeningStock";
            ClonedAdminSheetTable.Columns["CostPrice"].ColumnName = "SagePrice";
            ClonedAdminSheetTable.Columns["PO Cost"].ColumnName = "SheetPrice";
            ClonedAdminSheetTable.Columns.Add("Notes");
            ClonedAdminSheetTable.AcceptChanges();

            foreach (DataRow row in ClonedAdminSheetTable.Rows)
            {
                if (row["SagePrice"] == DBNull.Value && row["SheetPrice"] != DBNull.Value)
                {
                    row.Delete();
                }
                else if (row["SagePrice"] != DBNull.Value && row["SheetPrice"] == DBNull.Value)
                {
                    row.Delete();
                }
                else if (row["SagePrice"] == DBNull.Value && row["SheetPrice"] == DBNull.Value)
                {
                    row.Delete();
                }
                else
                {
                    var Sage2Digits = Math.Round(Convert.ToDouble(row["SagePrice"]), 2);
                    if (Sage2Digits > Math.Round(Convert.ToDouble(row["SheetPrice"]), 2))
                    {
                        row["SagePrice"] = Sage2Digits;
                        row["OpeningStock"] = Math.Round(Convert.ToDouble(row["OpeningStock"]));
                        row["Notes"] = "Lower";
                    }
                    else if (Sage2Digits < Math.Round(Convert.ToDouble(row["SheetPrice"]), 2))
                    {
                        row["SagePrice"] = Sage2Digits;
                        row["OpeningStock"] = Math.Round(Convert.ToDouble(row["OpeningStock"]));
                        row["Notes"] = "Higher";
                    }
                    else
                    {
                        row.Delete();
                    }
                }

            }
            ClonedAdminSheetTable.Columns.Remove("Warehouse");
            ClonedAdminSheetTable.AcceptChanges();
            TableHeaderBlock.Visibility = Visibility.Visible;
            CostPriceBatchGrid.ItemsSource = ClonedAdminSheetTable.DefaultView;
            CostPriceBatchGrid.Columns[5].Visibility = Visibility.Collapsed;
            CostPriceBatchGrid.Columns[7].Visibility = Visibility.Collapsed;
            if (ClonedAdminSheetTable.Rows.Count == 0)
            {
                CheckMessageBlock.Visibility = Visibility.Visible;
            }

            System.Windows.MessageBox.Show("Done");

        }

        private void CostPriceCheckerITEM()
        {
            System.Windows.MessageBox.Show("Please close Admin Stock Sheet (if open) before continuing");

            String filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Admin\\Admin Stock NEW.xlsx";

            DataTable AdminSheetTable = new DataTable("AdminSheetTable");
            DataTable SupplierOfficeTable = new DataTable();
            OleDbConnection oleExcelConnection = default(OleDbConnection);

            var Connection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
            oleExcelConnection = new OleDbConnection(Connection);

            using (OleDbCommand _cmd = new OleDbCommand())
            {
                _cmd.Connection = oleExcelConnection;
                _cmd.Parameters.AddWithValue("@Code", currentCode);
                _cmd.CommandText = "SELECT Batch, [Product Code], [PO Cost], [Quantity], [Warehouse] FROM [HACKLINGS STOCK$] " +
                    "WHERE [Product Code] = @Code AND Batch IS NOT NULL " +
                    "UNION ALL " +
                    "SELECT Batch, [Product Code], [PO Cost], [Quantity], [Warehouse] FROM [SUPPLIER STOCK$] " +
                    "WHERE [Product Code] = @Code AND Batch IS NOT NULL " +
                    "UNION ALL " +
                    "SELECT Batch, [Product Code], [PO Cost], [Quantity], [Warehouse] FROM [OFFICE$] " +
                    "WHERE [Product Code] = @Code AND Batch IS NOT NULL";

                using (OleDbDataAdapter _dap = new OleDbDataAdapter())
                {
                    _dap.SelectCommand = _cmd;
                    _dap.Fill(AdminSheetTable);

                    //System.Windows.MessageBox.Show("Done");
                }
            }
            AdminSheetTable.Columns.Add("ID");
            foreach (DataRow row in AdminSheetTable.Rows)
            {
                if (row["Warehouse"].ToString() == "Hacks" || row["Warehouse"].ToString() == "Hack" || row["Warehouse"].ToString() == "Hacklings")
                {
                    row["ID"] = row["Batch"].ToString() + "Hacklings";
                }
                else if (row["Warehouse"].ToString() == "Office")
                {
                    row["ID"] = row["Batch"].ToString() + "Office - Witney";
                }
                else
                {
                    row["ID"] = row["Batch"].ToString() + row["Warehouse"].ToString();
                }

            }

            List<string> AdminSheetBatchList = new List<string>();
            foreach (DataRow row in AdminSheetTable.Rows)
            {
                if (AdminSheetBatchList.Contains(row["ID"].ToString()))
                {
                    row.Delete();
                }
                else
                {
                    AdminSheetBatchList.Add(row["ID"].ToString());
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
                    _cmd.Parameters.AddWithValue("@Code", currentCode);
                    _dap.Fill(SageItemTable);

                }

                SageItemTable.Columns.Add("ID");
                SageItemTable.Columns["Name1"].ColumnName = "Location";
                foreach(DataRow row in SageItemTable.Rows)
                {
                    if (row["MovementReference"].ToString().Length == 10 && row["MovementReference"].ToString().Substring(0, 4) == "0000")
                    {
                        row["MovementReference"] = row["MovementReference"].ToString().Substring(4, 6);
                        row["ID"] = row["MovementReference"].ToString() + row["Location"].ToString();
                    }
                    else
                    {
                        row["ID"] = row["MovementReference"].ToString() + row["Location"].ToString();
                    }
                }

                List<string> BatchList = new List<string>();

                foreach (DataRow row in SageItemTable.Rows)
                {
                    if (BatchList.Contains(row["ID"].ToString()))
                    {
                        row.Delete();
                    }
                    else
                    {
                        BatchList.Add(row["ID"].ToString());
                    }
                }
            }
            SageItemTable.Columns.Remove("ItemID");
            SageItemTable.Columns.Remove("DateTimeCreated");

            SageItemTable.Columns.Remove("Name");
            SageItemTable.Columns.Remove("Description");
            SageItemTable.Columns.Remove("FreeStockQuantity");
            SageItemTable.Columns.Remove("StockUnitName");
            SageItemTable.Columns.Remove("AverageBuyingPrice");
            SageItemTable.Columns.Remove("Weight");
            SageItemTable.Columns["MovementReference"].SetOrdinal(0);
            //SageItemTable.Columns["OpeningStockLevel"];
            AdminSheetTable.Columns["Batch"].ColumnName = "MovementReference";
            AdminSheetTable.Columns["Quantity"].ColumnName = "SheetQtyLng";
            SageItemTable.AcceptChanges();
            AdminSheetTable.AcceptChanges();

            DataTable ClonedAdminSheetTable = AdminSheetTable.Clone();
            ClonedAdminSheetTable.Columns[3].DataType = typeof(decimal);
            foreach (DataRow dr in AdminSheetTable.Rows)
            {
                ClonedAdminSheetTable.ImportRow(dr);
            }


            SageItemTable.PrimaryKey = new DataColumn[] { SageItemTable.Columns["ID"] };
            ClonedAdminSheetTable.PrimaryKey = new DataColumn[] { ClonedAdminSheetTable.Columns["ID"] };
            ClonedAdminSheetTable.Merge(SageItemTable);

            ClonedAdminSheetTable.Columns.Remove("Code");
            ClonedAdminSheetTable.Columns.Remove("Product Code");
            ClonedAdminSheetTable.Columns["MovementReference"].ColumnName = "Batch Longer";
            ClonedAdminSheetTable.Columns["CostPrice"].SetOrdinal(1);
            ClonedAdminSheetTable.Columns["OpeningStockLevel"].SetOrdinal(2);
            ClonedAdminSheetTable.Columns["OpeningStockLevel"].ColumnName = "OpeningStock";
            ClonedAdminSheetTable.Columns["CostPrice"].ColumnName = "SagePrice";
            ClonedAdminSheetTable.Columns["PO Cost"].ColumnName = "SheetPrice";
            ClonedAdminSheetTable.Columns.Add("Notes");
            ClonedAdminSheetTable.AcceptChanges();

            foreach (DataRow row in ClonedAdminSheetTable.Rows)
            {
                if (row["SagePrice"] == DBNull.Value && row["SheetPrice"] != DBNull.Value)
                {
                    row.Delete();
                }
                else if (row["SagePrice"] != DBNull.Value && row["SheetPrice"] == DBNull.Value)
                {
                    row.Delete();
                }
                else if (row["SagePrice"] == DBNull.Value && row["SheetPrice"] == DBNull.Value)
                {
                    row.Delete();
                }
                else
                {
                    var Sage2Digits = Math.Round(Convert.ToDouble(row["SagePrice"]), 2);
                    if (Sage2Digits > Math.Round(Convert.ToDouble(row["SheetPrice"]), 2))
                    {
                        row["SagePrice"] = Sage2Digits;
                        row["OpeningStock"] = Math.Round(Convert.ToDouble(row["OpeningStock"]));
                        row["Notes"] = "Lower";
                    }
                    else if (Sage2Digits < Math.Round(Convert.ToDouble(row["SheetPrice"]), 2))
                    {
                        row["SagePrice"] = Sage2Digits;
                        row["OpeningStock"] = Math.Round(Convert.ToDouble(row["OpeningStock"]));
                        row["Notes"] = "Higher";
                    }
                    else
                    {
                        row.Delete();
                    }
                }

            }
            ClonedAdminSheetTable.Columns.Remove("Warehouse");
            ClonedAdminSheetTable.AcceptChanges();
            TableHeaderBlock.Visibility = Visibility.Visible;
            CostPriceBatchGrid.ItemsSource = ClonedAdminSheetTable.DefaultView;
            CostPriceBatchGrid.Columns[5].Visibility = Visibility.Collapsed;
            CostPriceBatchGrid.Columns[7].Visibility = Visibility.Collapsed;
            if (ClonedAdminSheetTable.Rows.Count == 0)
            {
                CheckMessageBlock.Visibility = Visibility.Visible;
            }

            System.Windows.MessageBox.Show("Done");

        }*/

        private void CostPriceCheckerALL()
        {
            System.Windows.MessageBox.Show("Please close Admin Stock Sheet (if open) before continuing");
            String filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Admin\\Admin Stock NEW.xlsx";
            System.Data.DataTable AdminSheetTable = new System.Data.DataTable("AdminSheetTable");
            OleDbConnection oleExcelConnection = default(OleDbConnection);

            var Connection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
            oleExcelConnection = new OleDbConnection(Connection);

            using (OleDbCommand _cmd = new OleDbCommand())
            {
                _cmd.Connection = oleExcelConnection;
                _cmd.CommandText = "SELECT Batch, [Product Code], [PO Cost], [Quantity], [Warehouse] FROM [HACKLINGS STOCK$] " +
                    "WHERE Batch IS NOT NULL AND [Warehouse] IS NOT NULL " +
                    "UNION ALL " +
                    "SELECT Batch, [Product Code], [PO Cost], [Quantity], [Warehouse] FROM [SUPPLIER STOCK$] " +
                    "WHERE Batch IS NOT NULL AND [Warehouse] IS NOT NULL " +
                    "UNION ALL " +
                    "SELECT Batch, [Product Code], [PO Cost], [Quantity], [Warehouse] FROM [OFFICE$] " +
                    "WHERE Batch IS NOT NULL AND [Warehouse] IS NOT NULL";

                using (OleDbDataAdapter _dap = new OleDbDataAdapter())
                {
                    _dap.SelectCommand = _cmd;
                    _dap.Fill(AdminSheetTable);

                    //System.Windows.MessageBox.Show("Done");
                }
            }
            //Create unique ID for each row (for Primary Key/Comparison)
            AdminSheetTable.Columns.Add("ID");
            foreach (DataRow row in AdminSheetTable.Rows)
            {
                if (row["Warehouse"].ToString().Trim() == "Hacks" || row["Warehouse"].ToString().Trim() == "Hack" || row["Warehouse"].ToString().Trim() == "Hacklings")
                {
                    row["ID"] = row["Product Code"].ToString().Trim().ToUpper() + row["Batch"].ToString() + "Hacklings";
                    row["Warehouse"] = "Hacklings";
                }
                else if (row["Warehouse"].ToString().Trim() == "Office")
                {
                    row["ID"] = row["Product Code"].ToString().Trim().ToUpper() + row["Batch"].ToString() + "Office - Witney";
                    row["Warehouse"] = "Office - Witney";
                }
                else if (row["Warehouse"].ToString().Trim() == "Polystar")
                {
                    row["ID"] = row["Product Code"].ToString().Trim().ToUpper() + row["Batch"].ToString() + "Polystar Plastics";
                    row["Warehouse"] = "Polystar Plastics";
                }
                else if (row["Warehouse"].ToString().Trim() == "PP")
                {
                    row["ID"] = row["Product Code"].ToString().Trim().ToUpper() + row["Batch"].ToString() + "Printed Polythene";
                    row["Warehouse"] = "Printed Polythene";
                }
                else
                {
                    row["ID"] = row["Product Code"].ToString().Trim().ToUpper() + row["Batch"].ToString().Trim() + row["Warehouse"].ToString().Trim();
                }

            }
            //Clear any duplicated lines (unlikely)
            List<string> AdminSheetIDList = new List<string>();
            foreach (DataRow row in AdminSheetTable.Rows)
            {
                if (AdminSheetIDList.Contains(row["ID"].ToString()))
                {
                    row.Delete();
                }
                else
                {
                    AdminSheetIDList.Add(row["ID"].ToString());
                }
            }
            AdminSheetTable.AcceptChanges();

            System.Data.DataTable SageTable = new System.Data.DataTable("SageItemTable");
            System.Data.DataTable SagePriceTable = new System.Data.DataTable("SagePriceTable");

            using (SqlConnection _con = new SqlConnection(connectionstring))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.SageItemCrossCheck;

                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    _dap.Fill(SageTable);

                }

            }

            using (SqlConnection _con = new SqlConnection(connectionstring))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.SageBatchCostPrice;

                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    _dap.Fill(SagePriceTable);

                }

            }
            //Add ID and quantity for Sage values
            SageTable.Columns.Add("ID");
            SageTable.Columns.Add("SageQty");
            foreach (DataRow row in SageTable.Rows)
            {
                if (row["IdentificationNo"].ToString().Count() >= 9)
                {
                    if (row["IdentificationNo"].ToString().Substring(0, 4) == "0000")
                    {
                        row["ID"] = row["Code"].ToString().ToUpper() + row["IdentificationNo"].ToString().Substring(4) + row["WarehouseName"].ToString();
                        row["SageQty"] = Convert.ToDouble(row["GoodsInQuantity"]) - Convert.ToDouble(row["GoodsOutQuantity"]);
                    }
                    else
                    {
                        row["ID"] = row["Code"].ToString().ToUpper() + row["IdentificationNo"].ToString() + row["WarehouseName"].ToString();
                        row["SageQty"] = Convert.ToDouble(row["GoodsInQuantity"]) - Convert.ToDouble(row["GoodsOutQuantity"]);
                    }
                }
                else if (row["IdentificationNo"].ToString().Count() < 9 && row["IdentificationNo"].ToString().Count() >= 6)
                {
                    row["ID"] = row["Code"].ToString().ToUpper() + row["IdentificationNo"].ToString() + row["WarehouseName"].ToString();
                    row["SageQty"] = Convert.ToDouble(row["GoodsInQuantity"]) - Convert.ToDouble(row["GoodsOutQuantity"]);
                }
                else
                {
                    row.Delete();
                }
            }
            SageTable.Columns.Remove("GoodsInQuantity");
            SageTable.Columns.Remove("GoodsOutQuantity");
            SageTable.AcceptChanges();

            //Clear any duplicated lines (more likely)
            List<string> SageIDList = new List<string>();
            foreach (DataRow row in SageTable.Rows)
            {
                if (SageIDList.Contains(row["ID"].ToString()))
                {
                    row.Delete();
                }
                else
                {
                    SageIDList.Add(row["ID"].ToString());
                }
            }
            SageTable.AcceptChanges();

            //Standardising batch numbers
            foreach (DataRow row in SageTable.Rows)
            {
                if (row["IdentificationNo"].ToString().Count() > 4)
                {
                    if (row["IdentificationNo"].ToString().Substring(0, 4) == "0000")
                    {
                        row["IdentificationNo"] = row["IdentificationNo"].ToString().Substring(4);
                    }
                }
            }

            AdminSheetTable.Columns.Add("Sage Batch");
            AdminSheetTable.Columns.Add("Sage Qty");
            AdminSheetTable.Columns.Add("Sage Location");

            //Removing sold out entries
            foreach (DataRow row in AdminSheetTable.Rows)
            {
                if (row["Warehouse"].ToString().Trim().ToUpper() == "SOLD OUT")
                {
                    row.Delete();
                }
            }
            AdminSheetTable.AcceptChanges();

            //Manually Merging based on certain criteria
            foreach (DataRow row in AdminSheetTable.Rows)
            {
                for (int i = 0; i < SageTable.Rows.Count; i++)
                {
                    DataRow SageRow = SageTable.Rows[i];

                    if (row["ID"].ToString() == SageRow["ID"].ToString()) //direct matches
                    {
                        row["Sage Batch"] = SageRow["IdentificationNo"];
                        row["Sage Qty"] = SageRow["SageQty"];
                        row["Sage Location"] = SageRow["WarehouseName"];
                        break;
                    }

                    else if (row["Batch"].ToString().Trim().ToUpper() == SageRow["IdentificationNo"].ToString().ToUpper() && row["Quantity"].ToString() == SageRow["SageQty"].ToString() && row["ID"].ToString() != SageRow["ID"].ToString())
                    {
                        row["Sage Batch"] = SageRow["IdentificationNo"];
                        row["Sage Qty"] = SageRow["SageQty"];
                        row["Sage Location"] = SageRow["WarehouseName"];
                        break;

                    }
                    else if (row["Batch"].ToString().Trim().ToUpper() == SageRow["IdentificationNo"].ToString().ToUpper() && row["Warehouse"].ToString().Trim().ToUpper() == SageRow["WarehouseName"].ToString().ToUpper() && row["ID"].ToString() != SageRow["ID"].ToString())
                    {
                        row["Sage Batch"] = SageRow["IdentificationNo"];
                        row["Sage Qty"] = SageRow["SageQty"];
                        row["Sage Location"] = SageRow["WarehouseName"];
                        break;
                    }
                }
            }
            AdminSheetTable.AcceptChanges();
            AdminSheetTable.Columns.Add("SagePrice");

            //Adding in sage cost after minimising size of table
            foreach (DataRow row in AdminSheetTable.Rows)
            {
                for (int i = 0; i < SagePriceTable.Rows.Count; i++)
                {
                    DataRow SagePriceRow = SagePriceTable.Rows[i];

                    if ("0000" + row["Batch"].ToString() == SagePriceRow["MovementReference"].ToString())
                    {
                        row["SagePrice"] = Math.Round(Convert.ToDouble(SagePriceRow["CostPrice"]), 2);

                    }
                }
                if (row["PO Cost"] != DBNull.Value)
                {
                    row["PO Cost"] = Math.Round(Convert.ToDecimal(row["PO Cost"]), 2);
                }
            }
            //remove lines with 0 for Sage Price, most of these are uninvoiced so blergh not point having 'em
            foreach (DataRow row in AdminSheetTable.Rows)
            {
                if (row["SagePrice"].ToString() == "0" || row["SagePrice"] == DBNull.Value)
                {
                    row.Delete();
                }
            }
            AdminSheetTable.AcceptChanges();

            //remove lines that match
            foreach (DataRow row in AdminSheetTable.Rows)
            {
                if (row["PO Cost"] != DBNull.Value)
                {
                    if (row["Quantity"].ToString() == row["Sage Qty"].ToString() && row["PO Cost"].ToString() == row["SagePrice"].ToString() && row["Warehouse"].ToString() == row["Sage Location"].ToString())
                    {
                        row.Delete();
                    }
                }
            }

            AdminSheetTable.AcceptChanges();
            CostPriceBatchGrid.ItemsSource = AdminSheetTable.DefaultView;
            CostPriceBatchGrid.Columns[5].Visibility = Visibility.Collapsed;
            CostPriceBatchGrid.Columns[6].Visibility = Visibility.Collapsed;
            CostPriceBatchGrid.Columns[1].Header = " Product Code ";
            CostPriceBatchGrid.Columns[2].Header = "Sheet Cost   ";
            CostPriceBatchGrid.Columns[3].Header = "Sheet Qty   ";
            CostPriceBatchGrid.Columns[4].Header = "Sheet Location   ";

            //Old merge below
            //SageTable.PrimaryKey = new DataColumn[] { SageTable.Columns["ID"] };
            //AdminSheetTable.PrimaryKey = new DataColumn[] { AdminSheetTable.Columns["ID"] };
            //AdminSheetTable.Merge(SageTable);
            //System.Windows.MessageBox.Show("Done");

        }

        private void BtnCheckALLPrice_Click(object sender, RoutedEventArgs e)
        {
            BtnCheckAllclicked = true;
            BtnCheckAllclicked2 = true;
            CostPriceBatchGrid.ItemsSource = null;
            SearchTextBox.Visibility = Visibility.Collapsed;
            SearchBorder.Visibility = Visibility.Collapsed;
            BtnSearch.Visibility = Visibility.Collapsed;
            ItemCodeBlock.Visibility = Visibility.Collapsed;

            timer.Tick += new EventHandler(SmoothCollapse_Tick);
            timer.Tick += new EventHandler(ButtonMove_Tick);
            timer.Interval = TimeSpan.FromMicroseconds(750);
            timer.Start();
            CostPriceCheckerALL();
            BtnCheckALLPrice.Visibility = Visibility.Collapsed;
            BtnResetAll.Visibility = Visibility.Visible;

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
            System.Data.DataTable CodeTable = new System.Data.DataTable();
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

        private void SmoothCollapse_Tick(object sender, EventArgs e)
        {
            if (BtnCheckAllclicked)
            {
                if (MainInfoPanel.Width > 0)
                {
                    MainInfoPanel.Width -= 2;
                    BatchInfoPanel.Width -= 2;
                }
                else
                {

                    MainInfoPanel.Visibility = Visibility.Collapsed;
                    BatchInfoPanel.Visibility = Visibility.Collapsed;
                    horizontalsep.Visibility = Visibility.Collapsed;
                    horizontalsep2.Visibility = Visibility.Collapsed;
                    BtnCheckAllclicked = false;
                }
            }
        }

        private void SmoothExpand_Tick(object sender, EventArgs e)
        {
            if (BtnResetclicked)
            {
                if (MainInfoPanel.Width < 300)
                {
                    MainInfoPanel.Width += 2;
                    BatchInfoPanel.Width += 2;
                }
                else
                {


                    BtnResetclicked = false;
                }
            }
        }
        private void SmoothGridMove_Tick(object sender, EventArgs e)
        {
            double WinWidth = this.ActualWidth;
            double WinHeight = this.ActualHeight;
            System.Windows.Thickness margin = CostComparePanel.Margin;
            System.Windows.Thickness marginBtn = BtnCheckALLPrice.Margin;
            System.Windows.Thickness marginSep = horizontalsep2.Margin;

            if (BtnSearchClicked)
            {
                if (CostComparePanel.Margin.Left < (WinWidth * 0.4))
                {
                    margin.Left += 2;
                    marginBtn.Left += 2;
                    marginSep.Left += 2;
                    CostComparePanel.Margin = margin;
                    BtnCheckALLPrice.Margin = marginBtn;
                    horizontalsep2.Margin = marginSep;
                }
                else
                {
                    CostComparePanel.Visibility = Visibility.Collapsed;
                    BtnCheckALLPrice.Visibility = Visibility.Collapsed;
                    horizontalsep2.Visibility = Visibility.Collapsed;
                    BtnSearchClicked = false;

                    BatchInfoPanel.Width = WinWidth * 0.65;
                    MainGraph.Height = WinHeight * 0.375;
                    timer.Stop();
                }
            }
        }

        private void SmoothGridMoveBack_Tick(object sender, EventArgs e)
        {
            double WinWidth = this.ActualWidth;
            double WinHeight = this.ActualHeight;
            System.Windows.Thickness margin = CostComparePanel.Margin;
            System.Windows.Thickness marginBtn = BtnCheckALLPrice.Margin;
            System.Windows.Thickness marginSep = horizontalsep2.Margin;

            if (BtnResetSearchClicked)
            {
                if (CostComparePanel.Margin.Left > 20)
                {
                    margin.Left -= 2;
                    marginBtn.Left -= 2;
                    marginSep.Left -= 2;
                    CostComparePanel.Margin = margin;
                    BtnCheckALLPrice.Margin = marginBtn;
                    horizontalsep2.Margin = marginSep;
                }
                else
                {
                    CostComparePanel.Visibility = Visibility.Visible;
                    BtnCheckALLPrice.Visibility = Visibility.Visible;
                    horizontalsep2.Visibility = Visibility.Visible;
                    BtnResetSearchClicked = false;

                    BatchInfoPanel.Width = 400;
                    MainGraph.Height = 200;
                    timer.Stop();
                }
            }
        }

        private void ButtonMove_Tick(object sender, EventArgs e)
        {
            System.Windows.Thickness margin = BtnCheckALLPrice.Margin;
            if (BtnCheckAllclicked2)
            {

                if (BtnCheckALLPrice.Margin.Left > 20)
                {
                    margin.Left -= 2;
                    BtnCheckALLPrice.Margin = margin;
                }
                else
                {
                    BtnCheckAllclicked2 = false;
                    timer.Stop();
                }
            }
        }

        private void ButtonExpand_Tick(object sender, EventArgs e)
        {
            System.Windows.Thickness margin = BtnCheckALLPrice.Margin;
            if (BtnResetclicked2)
            {

                if (BtnCheckALLPrice.Margin.Left < 500)
                {
                    margin.Left += 2;
                    BtnCheckALLPrice.Margin = margin;
                }
                else
                {
                    BtnResetclicked2 = false;
                    timer.Stop();
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

        private void BtnResetAll_Click(object sender, RoutedEventArgs e)
        {
            BtnResetclicked = true;
            BtnResetclicked2 = true;
            CostPriceBatchGrid.ItemsSource = null;
            MainInfoPanel.Visibility = Visibility.Visible;
            BatchInfoPanel.Visibility = Visibility.Visible;
            horizontalsep.Visibility = Visibility.Visible;
            horizontalsep2.Visibility = Visibility.Visible;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(SmoothExpand_Tick);
            timer.Tick += new EventHandler(ButtonExpand_Tick);
            timer.Interval = TimeSpan.FromMicroseconds(750);
            timer.Start();
            BtnCheckALLPrice.Visibility = Visibility.Visible;
            BtnResetAll.Visibility = Visibility.Collapsed;
            SearchTextBox.Visibility = Visibility.Visible;
            SearchBorder.Visibility = Visibility.Visible;
            BtnSearch.Visibility = Visibility.Visible;
            ItemCodeBlock.Visibility = Visibility.Visible;
        }

        private void BtnSearchReset_Click(object sender, RoutedEventArgs e)
        {
            currentCode = "";
            closestCode = "";
            codeMatch = false;
            BtnResetSearchClicked = true;
            currentLowestCompute = 99;
            RichTextDesc.Document.Blocks.Clear();
            RichTextSpec.Document.Blocks.Clear();
            RichTextFreeStock.Document.Blocks.Clear();
            RichTextUnit.Document.Blocks.Clear();
            RichTextWeight.Document.Blocks.Clear();
            RichTextAvPrice.Document.Blocks.Clear();
            RichTextTrendPrice.Document.Blocks.Clear();
            BatchGrid.ItemsSource = null;
            AllocatedBatchGrid.ItemsSource = null;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(SmoothGridMoveBack_Tick);
            timer.Interval = TimeSpan.FromMicroseconds(750);
            timer.Start();
            SearchTextBox.Text = "";
            BtnSearchReset.Visibility = Visibility.Collapsed;
            BtnSearch.Visibility = Visibility.Visible;
            BtnCheckALLPrice.Visibility = Visibility.Visible;
            MainGraph.Visibility = Visibility.Collapsed;
            DataContext = null;
        }

        private void GraphDataLoad(string currentCode)
        {
            MainGraph.Visibility = Visibility.Visible;

            double m1 = 0;
            double m2 = 0;
            double m3 = 0;
            double m4 = 0;
            double m5 = 0;
            double m6 = 0;
            double m7 = 0;
            double m8 = 0;
            double m9 = 0;
            double m10 = 0;
            double m11 = 0;
            double m12 = 0;

            System.Data.DataTable HistoryTable = new System.Data.DataTable();
            //String CodeTest = "PUK/MACHINE/004PCW";

            using (SqlConnection _con = new SqlConnection(connectionstring))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.ItemHistoryQuery;
                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    _cmd.Parameters.AddWithValue("@Code", currentCode);
                    _dap.Fill(HistoryTable);
                }
            }
            HistoryTable.Columns.Add("Month");
            HistoryTable.Columns.Add("Year");
            HistoryTable.AcceptChanges();

            List<string> YearList = new List<string>();

            foreach (DataRow row in HistoryTable.Rows)
            {
                DateTime LineTime = new DateTime();
                LineTime = DateTime.Parse(row["TransactionDate"].ToString());
                var ExtractedMonth = LineTime.Month;
                var ExtractedYear = LineTime.Year;
                row["Month"] = ExtractedMonth;
                row["Year"] = ExtractedYear;

                if (!YearList.Contains(row["Year"].ToString()))
                {
                    YearList.Add(row["Year"].ToString());
                }
            }
            int DividingNumber = YearList.Count;

            foreach(DataRow row in HistoryTable.Rows)
            {
                int MonthNumber = Convert.ToInt32(row["Month"]);
                switch (MonthNumber)
                {
                    case 1: 
                        m1 += Convert.ToDouble(row["Quantity"]) / DividingNumber;
                        break;
                    case 2:
                        m2 += Convert.ToDouble(row["Quantity"]) / DividingNumber;
                        break;
                    case 3:
                        m3 += Convert.ToDouble(row["Quantity"]) / DividingNumber;
                        break;
                    case 4:
                        m4 += Convert.ToDouble(row["Quantity"]) / DividingNumber;
                        break;
                    case 5:
                        m5 += Convert.ToDouble(row["Quantity"]) / DividingNumber;
                        break;
                    case 6:
                        m6 += Convert.ToDouble(row["Quantity"]) / DividingNumber;
                        break;
                    case 7:
                        m7 += Convert.ToDouble(row["Quantity"]) / DividingNumber;
                        break;
                    case 8:
                        m8 += Convert.ToDouble(row["Quantity"]) / DividingNumber;
                        break;
                    case 9:
                        m9 += Convert.ToDouble(row["Quantity"]) / DividingNumber;
                        break;
                    case 10:
                        m10 += Convert.ToDouble(row["Quantity"]) / DividingNumber;
                        break;
                    case 11:
                        m11 += Convert.ToDouble(row["Quantity"]) / DividingNumber;
                        break;
                    case 12:
                        m12 += Convert.ToDouble(row["Quantity"]) / DividingNumber;
                        break;
                }
            }


            MySeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Sold",
                    Values = new ChartValues<double> { m1, m2, m3, m4, m5, m6, m7, m8, m9, m10, m11, m12 },
                    LineSmoothness = 0.00

                }
            };
            BarLabels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Oct", "Sep", "Nov", "Dec", };
            // Bind the data context to this instance
            DataContext = this;
        }

    }
}
