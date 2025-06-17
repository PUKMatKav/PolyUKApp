using Azure;
using Microsoft.Data.SqlClient;
using PolyUKApp.SQL;
using PolyUKApp.SQL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
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
    /// Interaction logic for PODWindow.xaml
    /// </summary>
    public partial class PODWindow : Window
    {
        public PODWindow()
        {
            InitializeComponent();
            ComboItem();
        }

        private void BtnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            Close();
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

        public void ComboItem()
        {
            int CurrentYear = DateTime.Now.Year;
            ComboBoxYear.Text = " ";
            ComboBoxYear.Items.Add(CurrentYear);
            ComboBoxYear.Items.Add(CurrentYear - 1);

            ComboBoxMonth.Text = " ";
            ComboBoxMonth.Items.Add("January");
            ComboBoxMonth.Items.Add("February");
            ComboBoxMonth.Items.Add("March");
            ComboBoxMonth.Items.Add("April");
            ComboBoxMonth.Items.Add("May");
            ComboBoxMonth.Items.Add("June");
            ComboBoxMonth.Items.Add("July");
            ComboBoxMonth.Items.Add("August");
            ComboBoxMonth.Items.Add("September");
            ComboBoxMonth.Items.Add("October");
            ComboBoxMonth.Items.Add("November");
            ComboBoxMonth.Items.Add("December");

        }

        private void ComboBoxYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxYear.Text != "")
            {
                ComboBoxMonth.Visibility = Visibility.Visible;
                TextBlockMonth.Visibility = Visibility.Visible;
            }
        }

        private void ComboBoxMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxMonth.Text != "")
            {
                ComboBoxSupplier.Visibility = Visibility.Visible;
                TextBlockSupplier.Visibility = Visibility.Visible;
                SupplierLoadSQL();
            }
        }

        private void ComboBoxSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PODQuerySQL();
            //PODInfoLoad();
        }

        public async Task SupplierLoad()
        {

            String CurrentUser = Environment.UserName;
            String YearSelected = ComboBoxYear.Text.Substring(2, 2);
            await Task.Delay(100);
            String MonthSelected = ComboBoxMonth.Text.Substring(0, 3);
            String filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Finance - Documents\\Invoiced Figures\\YE Dec " + YearSelected + "\\Invoiced " + MonthSelected + " " + YearSelected + ".xlsx";
            System.Windows.MessageBox.Show(filepath);

            //Connection to specific Excel sheet

            DataTable SupplierTable = new DataTable();
            OleDbConnection oleExcelConnection = default(OleDbConnection);

            var Connection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
            oleExcelConnection = new OleDbConnection(Connection);

            using (OleDbCommand command = new OleDbCommand())
            {
                command.Connection = oleExcelConnection;
                command.CommandText = "SELECT * FROM [Invoiced$]";

                using (OleDbDataAdapter dap = new OleDbDataAdapter())
                {
                    dap.SelectCommand = command;
                    dap.Fill(SupplierTable);
                    var columnKeep = new List<String>() { "Sales Order No#", "Customer", "Supplier", "Haulier", "Signed POD", "BRC", "COC Received" }; //Columns needed
                    var columnRemove = new List<DataColumn>();

                    foreach (DataColumn column in SupplierTable.Columns)
                    {
                        if (!columnKeep.Any(name => column.ColumnName == name))
                        {
                            columnRemove.Add(column);
                        }
                    }
                    columnRemove.ForEach(col => SupplierTable.Columns.Remove(col));

                    SupplierTable.AcceptChanges();
                    List<string> SupplierList = new List<string>();

                    //Take suppliers with outstanding PODs and add to combobox
                    foreach (DataRow row in SupplierTable.Rows)
                    {
                        if (row["Supplier"] != DBNull.Value && row["Haulier"].ToString() == "DD" && row["Signed POD"].ToString() == "")
                        {
                            if (SupplierList.Contains(row["Supplier"].ToString()))
                            {

                            }
                            else
                            {
                                SupplierList.Add(row["Supplier"].ToString());
                            }
                        }
                    }
                    SupplierList.Sort();
                    ComboBoxSupplier.Items.Clear();
                    foreach (var Supplier in SupplierList)
                    {
                        ComboBoxSupplier.Items.Add(Supplier);
                    }
                }

            }
        }

        public async Task PODInfoLoad()
        {
            await Task.Delay(100);
            String CurrentUser = Environment.UserName;
            String YearSelected = ComboBoxYear.Text.Substring(2, 2);
            String MonthSelected = ComboBoxMonth.Text.Substring(0, 3);
            String filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Finance - Documents\\Invoiced Figures\\YE Dec " + YearSelected + "\\Invoiced " + MonthSelected + " " + YearSelected + ".xlsx";
            //System.Windows.MessageBox.Show(filepath);

            //Connection to specific Excel sheet

            DataTable SupplierTable = new DataTable();
            DataTable SupplierTableFiltered = new DataTable();
            OleDbConnection oleExcelConnection = default(OleDbConnection);

            var Connection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
            oleExcelConnection = new OleDbConnection(Connection);

            using (OleDbCommand command = new OleDbCommand())
            {
                command.Connection = oleExcelConnection;
                command.CommandText = "SELECT * FROM [Invoiced$]";

                using (OleDbDataAdapter dap = new OleDbDataAdapter())
                {
                    dap.SelectCommand = command;
                    dap.Fill(SupplierTable);
                    var columnKeep = new List<String>() { "Sales Order No#", "Customer", "Supplier", "Haulier", "Signed POD", "BRC", "COC Received" }; //Columns needed
                    var columnRemove = new List<DataColumn>();

                    foreach (DataColumn column in SupplierTable.Columns)
                    {
                        if (!columnKeep.Any(name => column.ColumnName == name))
                        {
                            columnRemove.Add(column);
                        }
                    }
                    columnRemove.ForEach(col => SupplierTable.Columns.Remove(col));

                    SupplierTable.AcceptChanges();
                }

            }
            SupplierTableFiltered.Clear();
            SupplierTableFiltered.Columns.Add("Sales Order No#");
            SupplierTableFiltered.Columns.Add("Customer");
            SupplierTableFiltered.Columns.Add("Supplier");
            SupplierTableFiltered.Columns.Add("Haulier");
            SupplierTableFiltered.Columns.Add("Signed POD");
            SupplierTableFiltered.Columns.Add("BRC");
            SupplierTableFiltered.Columns.Add("COC Received");
            /*foreach (DataRow fullrow in SupplierTable.Rows)
            {
                if ()
                SupplierTableFiltered.Rows.Add(fullrow["Sales Order No#"]);
            }*/
            System.Windows.MessageBox.Show("Finsihed");

        }

        public async Task PODQuerySQL()
        {
            await Task.Delay(100);
            var connectionString = DataAccess.GlobalSQL.Connection;
            DataTable PODTable = new DataTable();
            
            //load PODs

            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.PODQuery;
                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    _dap.Fill(PODTable);


                }

            }
            System.Windows.MessageBox.Show("Done");
        }

        public async Task SupplierLoadSQL()
        {
            await Task.Delay(100);
            var connectionString = DataAccess.GlobalSQL.Connection;
            String YearSelected = ComboBoxYear.Text;
            String MonthSelected = ComboBoxMonth.Text.Substring(0, 3);
            int MonthNumber = DateTime.ParseExact(MonthSelected, "MMM", CultureInfo.CurrentCulture).Month;
            String MonthNumSQL = "";
            if (MonthNumber < 10)
            {
                MonthNumSQL = "0" + MonthNumber.ToString();
            }
            if (MonthNumSQL == "01" | MonthNumSQL == "03" | MonthNumSQL == "05" | MonthNumSQL == "07" | MonthNumSQL == "08" | MonthNumSQL == "10" | MonthNumSQL == "12")
            {

            }
            else if (MonthNumSQL == "02")
            {

            }
            else
            {

            }

            String SQLDateEnd = MonthNumSQL + "/" + "31" + "/" + YearSelected; 
            DataTable SupplierTable = new DataTable();

            //get list of PODs saved currently
            var CurrentUser = Environment.UserName;
            var Filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Accounts - Documents\\PODS 2020";

            string[] PODArray = Directory.GetFiles(Filepath, "*.*", SearchOption.AllDirectories);
            String PODFIles = String.Concat(PODArray);

            //load Suppliers based on above
            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.PODQuery;
                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);

                    //_cmd.Parameters.AddWithValue("ConfirmedYear", YearSelected);
                    //_cmd.Parameters.AddWithValue("ConfirmedMonth", MonthSelected);

                    _dap.Fill(SupplierTable);
                    SupplierTable.Columns.Add("POD");

                    //foreach (DataRow baseRow in SupplierTable.Rows)
                    //{
                    //    if (PODFIles.Contains(baseRow["DocumentNo"].ToString().Substring(4, 6)))
                    //    {
                    //        baseRow["POD"] = "YES";
                    //    }
                    //}

                    //List<string> SupplierList = new List<string>();

                    //foreach (DataRow row in SupplierTable.Rows)
                    //{
                    //    if (row["SupplierAccountNumber"] != DBNull.Value)
                    //    {
                    //        if (SupplierList.Contains(row["SupplierAccountNumber"].ToString()) && PODFIles.Contains(row["DocumentNo"].ToString()))
                    //        {
                    //            System.Windows.MessageBox.Show("POD found");
                    //        }
                    //        else
                    //        {
                    //            SupplierList.Add(row["SupplierAccountNumber"].ToString());
                    //        }
                    //    }
                    //}
                    //SupplierList.Sort();
                    //ComboBoxSupplier.Items.Clear();
                    //foreach (var Supplier in SupplierList)
                    //{
                    //    ComboBoxSupplier.Items.Add(Supplier);
                    //}
                }
            }
        }
    }
}
