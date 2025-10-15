using Azure;
using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using PolyUKApp.SQL;
using PolyUKApp.SQL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
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
        String MonthSelected = "";
        String YearSelected = "";
        DataTable SupplierTable = new DataTable();
        DataTable DataSheetTable = new DataTable();
        DataTable DataSheetSavedTable = new DataTable();

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
                BtnExport.Visibility = Visibility.Visible;
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
            if (ComboBoxSupplier.Text == "")
            {
                DataGrid1.ItemsSource = null;
            }
            else
            {
                var connectionString = DataAccess.GlobalSQL.Connection;
                String supplierCode = ComboBoxSupplier.Text;
                //SupplierTable.Columns.Remove("");
                DataView supplierDataView = new DataView(SupplierTable);

                BindingSource bs = new BindingSource
                {
                    DataSource = supplierDataView.Table,
                    Filter = "[SupplierAccountNumber] like '%" + supplierCode + "%'"
                };

                DataGrid1.ItemsSource = bs;
                DataGrid1.Columns[1].Visibility = Visibility.Collapsed;
                DataGrid1.Columns[2].Visibility = Visibility.Collapsed;
                DataGrid1.Columns[4].Visibility = Visibility.Collapsed;
                DataGrid1.Columns[5].Visibility = Visibility.Collapsed;
                DataGrid1.Columns[7].Visibility = Visibility.Collapsed;
                DataGrid1.Columns[8].Visibility = Visibility.Collapsed;

            }

        }

        public async Task SupplierLoadSQL()
        {
            SupplierTable.Dispose();
            SupplierTable.Clear();
            DataGrid1.ItemsSource = null;
            await Task.Delay(100);
            var connectionString = DataAccess.GlobalSQL.Connection;
            YearSelected = ComboBoxYear.Text;
            MonthSelected = ComboBoxMonth.Text.Substring(0, 3);
            int MonthNumber = DateTime.ParseExact(MonthSelected, "MMM", CultureInfo.CurrentCulture).Month;
            String MonthNumSQL = "";
            String DaysMax = "";
            if (MonthNumber < 10)
            {
                MonthNumSQL = "0" + MonthNumber.ToString();
            }
            if (MonthNumSQL == "01" | MonthNumSQL == "03" | MonthNumSQL == "05" | MonthNumSQL == "07" | MonthNumSQL == "08" | MonthNumSQL == "10" | MonthNumSQL == "12")
            {
                DaysMax = "31";
            }
            else if (MonthNumSQL == "02")
            {
                DaysMax = "28";
            }
            else
            {
                DaysMax = "30";
            }

            String SQLDateEnd = MonthNumSQL + "/" + DaysMax + "/" + YearSelected;
            String SQLDateStart = MonthNumSQL + "/" + "01" + "/" + YearSelected;
            

            //get list of PODs saved currently
            var CurrentUser = Environment.UserName;
            var Filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Accounts - Documents\\PODS 2020";
            var filePathCoC = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\BRC &  ISO 2020\\BRCGS\\Coc & Data Sheets\\BRC SUPPLIER CERTIFICATES OF CONFORMITY";

            string[] PODArray = Directory.GetFiles(Filepath, "*.*", SearchOption.AllDirectories);
            String PODFIles = String.Concat(PODArray);

            string[] CoCArray = Directory.GetFiles(filePathCoC, "*.*", SearchOption.AllDirectories);
            String CoCFiles = String.Concat(CoCArray);

            //load Suppliers based on above
            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.PODSupplierQuery;
                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);

                    _cmd.Parameters.AddWithValue("MonthSelected", MonthSelected);
                    _cmd.Parameters.AddWithValue("YearSelected", YearSelected);

                    _dap.Fill(SupplierTable);

                    if (!SupplierTable.Columns.Contains("POD"))
                    {
                        SupplierTable.Columns.Add("POD");
                    }
                    if (!SupplierTable.Columns.Contains("BRC CoC"))
                    {
                        SupplierTable.Columns.Add("BRC CoC");
                    }

                    foreach (DataRow CoCRow in SupplierTable.Rows)
                    {
                        if (CoCFiles.Contains(CoCRow["DocumentNo"].ToString().Substring(4,6)))
                        {
                            CoCRow["BRC CoC"] = "SAVED";
                        }
                        else if (CoCRow["ItemCode"].ToString().Substring(0,3) == "BRC" && (!CoCFiles.Contains(CoCRow["DocumentNo"].ToString().Substring(4, 6))))
                        {
                            CoCRow["BRC CoC"] = "REQUIRED";
                        }
                        else
                        {
                            CoCRow["BRC CoC"] = "N/A";
                        }
                    }

                    foreach (DataRow baseRow in SupplierTable.Rows)
                    {
                        if (PODFIles.Contains(baseRow["DocumentNo"].ToString().Substring(4, 6)) || PODFIles.Contains(baseRow["TransactionReference"].ToString()))
                        {
                            baseRow["POD"] = "Saved";

                            if (baseRow["BRC CoC"].ToString() == "N/A" || baseRow["BRC CoC"].ToString() == "SAVED")
                            {
                                baseRow.Delete();
                            }
                        }
                        else
                        {
                            baseRow["POD"] = "Missing";
                        }
                    }
                    SupplierTable.AcceptChanges();
                    
                    List<string> SupplierList = new List<string>();

                    foreach (DataRow row in SupplierTable.Rows)
                    {
                        if (row["SupplierAccountNumber"] != DBNull.Value)
                        {
                            if (SupplierList.Contains(row["SupplierAccountNumber"].ToString()))
                            {
                                
                            }
                            else
                            {
                                SupplierList.Add(row["SupplierAccountNumber"].ToString());
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
            await Task.Delay(100);
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            string tempResult = System.IO.Path.GetTempPath();
            string filePath = tempResult + "PODs.xlsx";
            DataTable exportPODs = new DataTable();
            exportPODs = SupplierTable.Copy();

            foreach (DataRow row in exportPODs.Rows)
            {
                if (row["SupplierAccountNumber"].ToString() != ComboBoxSupplier.Text)
                {
                    row.Delete();
                }
            }
            exportPODs.Columns.Remove("ConfirmedMonth");
            exportPODs.Columns.Remove("ConfirmedYear");
            exportPODs.Columns.Remove("InvDueMonth");
            exportPODs.Columns.Remove("InvDueYear");
            exportPODs.Columns.Remove("ItemCode");
            exportPODs.Columns.Remove("SupplierAccountNumber");
            exportPODs.Columns[0].ColumnName = "Order Number";
            exportPODs.Columns[1].ColumnName = "Customer Name";
            exportPODs.Columns[2].ColumnName = "Supplier Invoice";
            exportPODs.AcceptChanges();
            exportPODs = exportPODs.DefaultView.ToTable(true);



            XLWorkbook wb = new XLWorkbook();
            bool answer = false;
            wb.AddWorksheet(exportPODs, "PODs");
            try
            {
                wb.SaveAs(filePath);
            }
            catch (IOException ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            var psi = new ProcessStartInfo(filePath) { UseShellExecute = true };
            Process.Start(psi);
        }

        private void PODBtn_Click(object sender, RoutedEventArgs e)
        {
            TextBlockStock.Text = "POD / CoC System";
            TxtSelectYr.Visibility = Visibility.Visible;
            ComboBoxYear.Visibility = Visibility.Visible;
            DataSearchBtn.Visibility = Visibility.Collapsed;
            PODBtn.Opacity = 0.5;
            DataBtn.Opacity = 1;
        }

        private void DataBtn_Click(object sender, RoutedEventArgs e)
        {
            TextBlockStock.Text = "Data Sheet System";
            TxtSelectYr.Visibility = Visibility.Collapsed;
            ComboBoxYear.Visibility = Visibility.Collapsed;
            ComboBoxYear.Text = " ";
            TextBlockMonth.Visibility = Visibility.Collapsed;
            ComboBoxMonth.Visibility = Visibility.Collapsed;
            ComboBoxMonth.Text = " ";
            TextBlockSupplier.Visibility = Visibility.Collapsed;
            ComboBoxSupplier.Visibility = Visibility.Collapsed;
            ComboBoxSupplier.Text = " ";
            DataSearchBtn.Visibility = Visibility.Visible;
            DataGrid1.ItemsSource = null;
            PODBtn.Opacity = 1;
            DataBtn.Opacity = 0.5;
        }

        private void DataSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            DataSheetTable.Dispose();
            DataSheetTable.Clear();
            DataGrid1.ItemsSource = null;
            var connectionString = DataAccess.GlobalSQL.Connection;

            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.BRCDataSheetCheck;
                _con.Open();

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);

                    _dap.Fill(DataSheetTable);

                }
            }
            //filter down to newest order only

            List<string> CodeList = new List<string>();

            foreach (DataRow row in DataSheetTable.Rows)
            {
                if (!CodeList.Contains(row[0].ToString()))
                {
                    CodeList.Add(row[0].ToString());
                }
                else
                {
                    row.Delete();
                }
            }
            DataSheetTable.AcceptChanges();

            //pull Data Sheets from folders for comparison


            DataSheetSavedTable.Columns.Add("Sup");
            DataSheetSavedTable.Columns.Add("Item");
            List<string> DataSheetList = new List<string>();
            var CurrentUser = Environment.UserName;
            var filePathDataSheets = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\BRC &  ISO 2020\\BRCGS\\Coc & Data Sheets\\BRC SUPPLIER  DATA SHEETS";
            DataSheetList = Directory.GetFiles(filePathDataSheets, "*.*", SearchOption.AllDirectories).ToList<string>();

            for (int i = 0; i < DataSheetList.Count; i++)
            {
                var Line = DataSheetList[i].Substring(131);
                if(Line.Contains("1. PUK"))
                {
                    if (!Line.ToUpper().Contains("ARCHIVED"))
                    {
                        DataSheetSavedTable.Rows.Add(Line.Substring(14).Split('\\'));
                    }
                }
                else
                {
                    if (!Line.ToUpper().Contains("ARCHIVED"))
                    {
                        DataSheetSavedTable.Rows.Add(Line.Split('\\'));
                    }
                }
            }



            //String DataSheetFiles = String.Concat(DataSheetArray);

            System.Windows.MessageBox.Show("Done");
        }
    }
}
