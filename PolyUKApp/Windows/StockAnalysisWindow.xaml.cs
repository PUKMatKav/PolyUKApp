using System;
using PolyUKApp.SQL;
using PolyUKApp.SQL.Models;
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
using System.Data;
using Microsoft.Data.SqlClient;

namespace PolyUKApp.Windows
{
    /// <summary>
    /// Interaction logic for StockAnalysisWindow.xaml
    /// </summary>
    public partial class StockAnalysisWindow : Window
    {
        List<Item> ItemCode = new List<Item>();
        String connectionstring = DataAccess.GlobalSQL.Connection;

        public StockAnalysisWindow()
        {
            InitializeComponent();
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
            LoadItemInfo();
            LoadBatchInfo();
            LoadAllocationInfo();
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


            String ItemCode = SearchTextBox.Text;
            DataTable ItemTable = new DataTable();

            using (SqlConnection _con = new SqlConnection(connectionstring))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.ItemAnalysisQuery;

                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    _cmd.Parameters.AddWithValue("@Code", ItemCode);
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
            String ItemCode = SearchTextBox.Text;
            DataTable BatchTable = new DataTable();

            using (SqlConnection _con = new SqlConnection(connectionstring))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.ItemBatchQuery;
                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap  =new SqlDataAdapter(_cmd);
                    _cmd.Parameters.AddWithValue("@Code", ItemCode);
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
            System.Windows.MessageBox.Show("Done");
        }

        private void LoadAllocationInfo()
        {
            String ItemCode = SearchTextBox.Text;
            DataTable AllocationTable = new DataTable();

            using (SqlConnection _con = new SqlConnection(connectionstring))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.ItemAllocatedBatchQuery;
                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    _cmd.Parameters.AddWithValue("@Code", ItemCode);
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
            System.Windows.MessageBox.Show("Done");
        }

    }
}
