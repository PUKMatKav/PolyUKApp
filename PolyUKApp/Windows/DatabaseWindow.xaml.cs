using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.ApplicationServices;
using MySql.Data.MySqlClient;
using PolyUKApp.SQL;
using PolyUKApp.SQL.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
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
using static PolyUKApp.Windows.CallTimeWindow;

namespace PolyUKApp.Windows
{
    /// <summary>
    /// Interaction logic for DatabaseWindow.xaml
    /// </summary>
    public partial class DatabaseWindow : Window
    {

        public DatabaseWindow()
        {
            InitializeComponent();
        }
        private void TopBar0_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void BtnSearchData_Click(object sender, RoutedEventArgs e)
        {
            string SearchColumn = ComboBoxSearch.Text;
            BindingSource bs = new BindingSource
            {
                DataSource = DataGrid1.ItemsSource,
                Filter = "[" + SearchColumn + "]" + " like '%" + TxtBxSearch.Text + "%'"
            };
            DataGrid1.ItemsSource = bs;
        }

        private void BtnResetData_Click(object sender, RoutedEventArgs e)
        {
            BindingSource bsR = new BindingSource
            {
                DataSource = DataGrid1.ItemsSource,
                Filter = ""
            };
            DataGrid1.ItemsSource = bsR;
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

        private void BtnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnCRMLoad_Click(object sender, RoutedEventArgs e)
        {
            DataGrid1.ItemsSource = null;
            SqlConnectCRM();
        }

        private void BtnIntakeLoad_Click(object sender, RoutedEventArgs e)
        {
            SqlConnectIntakeSheet();
        }
        private void BtnVanLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadMySql();
        }

        public void SqlConnectCRM()
        {
            string connectionString = DataAccess.GlobalSQL.ConnectionCRM;
            DataTable noCommsTable = new DataTable("NoCommsList");
            DataTable CommsTable = new DataTable("CommsList");


            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                string queryStatement = DataAccess.GlabalSQLQueries.CRMCompanies;
                string queryStatement2 = DataAccess.GlabalSQLQueries.CRMWithComms;

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {

                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);

                    _con.Open();
                    _dap.Fill(noCommsTable);
                    _con.Close();

                }
                using (SqlCommand _cmd2 = new SqlCommand(queryStatement2, _con))
                {
                    SqlDataAdapter _dap2 = new SqlDataAdapter(_cmd2);
                    _con.Open();
                    _dap2.Fill(CommsTable);
                    _con.Close();
                    //CommsTable = CommsTable.DefaultView.ToTable(true);
                }
            }
            List<string> IDList = new List<string>();
            List<DataRow> toDelete = new List<DataRow>();
            foreach (DataRow row in CommsTable.Rows)
            {
                IDList.Add(row[3].ToString());
            }

            foreach (DataRow noCommRow in noCommsTable.Rows)
            {
                if (IDList.Contains(noCommRow[0].ToString()))
                {
                    toDelete.Add(noCommRow);
                }
            }
            foreach (DataRow dr in toDelete)
            {
                noCommsTable.Rows.Remove(dr);
            }
            noCommsTable.AcceptChanges();
            DataGrid1.ItemsSource = noCommsTable.DefaultView;
        }

        public void SqlCRMProspectsNoComms()
        {
            string connectionString = DataAccess.GlobalSQL.ConnectionCRM;
            DataTable noCommsTable = new DataTable("NoCommsList");
            DataTable CommsTable = new DataTable("CommsList");


            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                string queryStatement = DataAccess.GlabalSQLQueries.CRMProspects;
                string queryStatement2 = DataAccess.GlabalSQLQueries.CRMWithComms;

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {

                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);

                    _con.Open();
                    _dap.Fill(noCommsTable);
                    _con.Close();

                }
                using (SqlCommand _cmd2 = new SqlCommand(queryStatement2, _con))
                {
                    SqlDataAdapter _dap2 = new SqlDataAdapter(_cmd2);
                    _con.Open();
                    _dap2.Fill(CommsTable);
                    _con.Close();
                    //CommsTable = CommsTable.DefaultView.ToTable(true);
                }
            }
            List<string> IDList = new List<string>();
            List<DataRow> toDelete = new List<DataRow>();
            foreach (DataRow row in CommsTable.Rows)
            {
                IDList.Add(row[3].ToString());
            }

            foreach (DataRow noCommRow in noCommsTable.Rows)
            {
                if (IDList.Contains(noCommRow[0].ToString()))
                {
                    toDelete.Add(noCommRow);
                }
            }
            foreach (DataRow dr in toDelete)
            {
                noCommsTable.Rows.Remove(dr);
            }
            noCommsTable.AcceptChanges();
            DataGrid1.ItemsSource = noCommsTable.DefaultView;
        }

        public void SqlCRMProspectsNoCommsJS()
        {
            string connectionString = DataAccess.GlobalSQL.ConnectionCRM;
            DataTable noCommsTable = new DataTable("NoCommsList");
            DataTable CommsTable = new DataTable("CommsList");


            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                string queryStatement = DataAccess.GlabalSQLQueries.CRMProspectsJS;
                string queryStatement2 = DataAccess.GlabalSQLQueries.CRMWithCommsJS;

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {

                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);

                    _con.Open();
                    _dap.Fill(noCommsTable);
                    _con.Close();

                }
                using (SqlCommand _cmd2 = new SqlCommand(queryStatement2, _con))
                {
                    SqlDataAdapter _dap2 = new SqlDataAdapter(_cmd2);
                    _con.Open();
                    _dap2.Fill(CommsTable);
                    _con.Close();
                    //CommsTable = CommsTable.DefaultView.ToTable(true);
                }
            }
            List<string> IDList = new List<string>();
            List<DataRow> toDelete = new List<DataRow>();
            foreach (DataRow row in CommsTable.Rows)
            {
                IDList.Add(row[3].ToString());
            }

            foreach (DataRow noCommRow in noCommsTable.Rows)
            {
                if (IDList.Contains(noCommRow[0].ToString()))
                {
                    toDelete.Add(noCommRow);
                }
            }
            foreach (DataRow dr in toDelete)
            {
                noCommsTable.Rows.Remove(dr);
            }
            noCommsTable.AcceptChanges();
            DataGrid1.ItemsSource = noCommsTable.DefaultView;
        }


        public void SqlConnectIntakeSheet()
        {
            string connectionString = "server=POLYSQL01\\sage; database=PolytheneUK_Sage200; Integrated Security=true; encrypt=false";
            DataTable commsTable = new DataTable("CommsList");

            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                string queryStatement = "SELECT [Order No.] AS OrderNo, Date, [Sales Person], [A/C Name] AS Account, [A/c Ref] as Code, [Confirmed Month], [SO Total £], [PO Total £] " +
                        "FROM dbo.SC_BI_SO_OutstandingOrders ";

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {

                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);

                    _con.Open();
                    _dap.Fill(commsTable);
                    _con.Close();
                    ComboBoxSearch.ItemsSource = commsTable.Columns;
                    DataGrid1.ItemsSource = null;
                    DataGrid1.ItemsSource = commsTable.DefaultView;

                }
            }
        }

        public async void BtnWOLoad_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = DataAccess.GlobalSQL.Connection;
            DataTable WOTable = new DataTable("WOList");
            DataTable WODetails = new DataTable("WODetailsList");
            //draw WOList information
            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                _con.Open();
                String queryStatement = DataAccess.GlabalSQLQueries.WOQuery;
                String queryStatement2 = DataAccess.GlabalSQLQueries.WODetails;

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    _cmd.Parameters.AddWithValue("@Status", "New");
                    _cmd.Parameters.AddWithValue("@Status1", "Issued");
                    _cmd.Parameters.AddWithValue("@Status2", "Allocated");
                    _dap.Fill(WOTable);
                }

                using (SqlCommand _cmd2 = new SqlCommand(queryStatement2, _con))
                {
                    SqlDataAdapter _dap2 = new SqlDataAdapter(_cmd2);
                    _cmd2.Parameters.AddWithValue("@WOStatus", "New");
                    _cmd2.Parameters.AddWithValue("@WOStatus1", "Issued");
                    _cmd2.Parameters.AddWithValue("@WOStatus2", "Allocated");
                    _dap2.Fill(WODetails); 
                }
                
                //narrow down data to 10 digit start and end date and WO number
                DataTable WODateTable = new DataTable("WODateTable");
                WODateTable.Columns.Add("WO UID");
                WODateTable.Columns.Add("WO Number");
                WODateTable.Columns.Add("Start Date");
                WODateTable.Columns.Add("Due Date");
                foreach (DataRow row in WOTable.Rows)
                {
                    WODateTable.Rows.Add(row["SiWorksOrderID"], row["WONumber"], row["StartDate"].ToString().Substring(0,10), row["DueDate"].ToString().Substring(0, 10));
                }

                DataTable WODetailsTable = new DataTable("WODetailsTable");
                WODetails.DefaultView.ToTable(true, "WONumber");
                WODetailsTable.Columns.Add("WO UID");
                WODetailsTable.Columns.Add("WO Number");
                WODetailsTable.Columns.Add("Promised Date");
                WODetailsTable.Columns.Add("Job Location");
                foreach(DataRow row in WODetails.Rows)
                {
                    object cellvalue = row["PromisedDeliveryDate"];
                    if (cellvalue == DBNull.Value)
                    {
                        WODetailsTable.Rows.Add(row["SiWorksOrderID"], row["WONumber"], "", row["WOType"]);
                    }
                    else
                    {
                        WODetailsTable.Rows.Add(row["SiWorksOrderID"], row["WONumber"].ToString().Substring(0,10), row["PromisedDeliveryDate"].ToString().Substring(0,10), row["WOType"]);
                    }
                }
                DataTable UniqueWOTable = WODetailsTable.DefaultView.ToTable(true);

                WODateTable.PrimaryKey = new DataColumn[] {WODateTable.Columns[0]};
                UniqueWOTable.PrimaryKey = new DataColumn[] {UniqueWOTable.Columns[0]};

                WODateTable.Merge(UniqueWOTable);
                WODateTable.Columns.Add("Dates");

                foreach(DataRow row in WODateTable.Rows)
                {
                    object WOStartDay = row["Start Date"].ToString().Substring(8,2);
                    object WOStartMonth = row["Start Date"].ToString().Substring(5, 2);
                    object WOStartYear = row["Start Date"].ToString().Substring(0, 4);
                    object WODueDay = row["Due Date"].ToString().Substring(8, 2);
                    object WODueMonth = row["Due Date"].ToString().Substring(5, 2);
                    object WODueYear = row["Due Date"].ToString().Substring(0, 4);
                    var WOSysDateStart = new DateTime(Convert.ToInt32(WOStartYear), Convert.ToInt32(WOStartMonth), Convert.ToInt32(WOStartDay));
                    var WOSysDateDue = new DateTime(Convert.ToInt32(WODueYear), Convert.ToInt32(WODueMonth), Convert.ToInt32(WODueDay));
                    
                    int daysint = (WOSysDateDue - WOSysDateStart).Days - 1;
                    int daysdiff = (WOSysDateDue - WOSysDateStart).Days;
                    if (daysdiff == 0 || daysdiff == 1)
                    {

                    }
                    else
                    {
                        //String ragestring = Enumerable.Range(1, daysint).Select(i => i.ToString()).ToArray();
                        List<String> range = Enumerable.Range(1, daysint)
                            .Select(i => WOSysDateStart.AddDays(i).ToString().Substring(0,10) + ".")
                            .ToList();
                            TestRich.AppendText(String.Join(Environment.NewLine, range));
                    }

                }

                    DataGrid1.ItemsSource = null;
                    DataGrid1.ItemsSource = WODateTable.DefaultView;
                    ComboBoxSearch.ItemsSource = WODateTable.Columns;
                    _con.Close();

            }
        }

        public void LoadMySql()
        {
            var connectionstring = DataAccess.GlobalSQL.ConnectionMySQLVan;
            DataTable dt = new DataTable();
            using (MySqlConnection _con = new MySqlConnection(connectionstring))
            {
                
                String QueryStatement = "SELECT * FROM collection_database";

                using (MySqlCommand _cmd = new MySqlCommand(QueryStatement, _con))
                {
                    MySqlDataAdapter _dap = new MySqlDataAdapter(_cmd);
                    _dap.Fill(dt);
                }
                DataGrid1.ItemsSource = null;
                DataGrid1.ItemsSource = dt.DefaultView;
            }

        }

        public void GetExcel()
        {
            string CurrentUser = Globals.Username;
            string filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Waste Collection\\2024 Collection List Database.xlsx;";
            string conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + @"Extended Properties='Excel 8.0;HDR=Yes;'";
            DataTable VanList = new DataTable();

            using (OleDbConnection _con = new OleDbConnection(conn))
            {

                using (OleDbCommand _cmd = new OleDbCommand("SELECT * from [Visits$]", _con))
                {
                    OleDbDataAdapter _dap = new OleDbDataAdapter(_cmd);

                    _con.Open();
                    _dap.Fill(VanList);
                    _con.Close();
                    ComboBoxSearch.ItemsSource = VanList.Columns;
                    DataGrid1.ItemsSource = null;
                    DataGrid1.ItemsSource = VanList.DefaultView;
                    
                }
            }
        }

        private void BtnVanSave_Click(object sender, RoutedEventArgs e)
        {
            string CurrentUser = Globals.Username;
            string filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Waste Collection\\2024 Collection List Database.xlsx;";
            string conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + @"Extended Properties='Excel 8.0;HDR=Yes;'";

            DataTable VanToSave = new DataTable();
            VanToSave = ((DataView)DataGrid1.ItemsSource).ToTable();


            using (OleDbConnection _con = new OleDbConnection(conn))
            {
                _con.Open();
                DataTable dt = _con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                foreach (DataRow row in VanToSave.Rows)

                {
                    object VanCoName = row["Company Name"];
                    object VanTown = row["Town"];
                    using (OleDbCommand _cmd = new OleDbCommand("UPDATE [Visits$] SET Town=@Town WHERE [Company Name]=COName", _con))
                    {

                        _cmd.Parameters.AddWithValue("@Town", VanTown.ToString());
                        _cmd.Parameters.AddWithValue("@COName", VanCoName.ToString());
                        _cmd.ExecuteNonQuery();
                    }
                }
                System.Windows.MessageBox.Show("Information Updated!");
            }
        }

        private void BtnCRMProspectNoComms_Click(object sender, RoutedEventArgs e)
        {
            DataGrid1.ItemsSource = null;
            SqlCRMProspectsNoCommsJS();
        }

        private void BtnCRMDupeAccounts_Click(object sender, RoutedEventArgs e)
        {
            DataGrid1.ItemsSource = null;
            SqlCRMShowDuplicateAccounts();
        }

        private void SqlCRMShowDuplicateAccounts()
        {
            string connectionString = DataAccess.GlobalSQL.ConnectionCRM;
            DataTable CompanyTable = new DataTable();
            DataTable DupeTable = new DataTable();
            DupeTable.Columns.Add("Comp_CompanyId");
            DupeTable.Columns.Add("Comp_Name");
            DupeTable.Columns.Add("comp_sc_salesperson");

            using (SqlConnection _con  = new SqlConnection(connectionString))
            {
                string queryStatement = DataAccess.GlabalSQLQueries.CRMCompaniesALL;

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);

                    _con.Open();
                    _dap.Fill(CompanyTable);
                    _con.Close();
                }
            }

            List<string> CompanyList = new List<string>();

            for (int i = 0; i < CompanyTable.Rows.Count; i++)
            {
                DataRow row = CompanyTable.Rows[i];
                String CompName = row["Comp_Name"].ToString().ToUpper().Trim();
                if (CompanyList.Contains(CompName))
                {
                    DupeTable.Rows.Add(row.ItemArray);
                }
                else
                {
                    CompanyList.Add(CompName);
                }
            }
            System.Windows.MessageBox.Show("Done");

        }

        private void SqlCRMJamesandJames()
        {
            string connectionString = DataAccess.GlobalSQL.ConnectionCRM;
            DataTable CRMTable = new DataTable();

            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                string queryStatement = DataAccess.GlabalSQLQueries.CommsJamesWandS;

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);

                    _con.Open();
                    _dap.Fill(CRMTable);
                    _con.Close();
                }
            }
        }

        private void BtnCRMCommLink_Click(object sender, RoutedEventArgs e)
        {
            SqlCRMJamesandJames();
        }

        private void SqlReportDebtorsList()
        {
            string connectionString = DataAccess.GlobalSQL.Connection;
            DataTable DebtorTable = new DataTable();

            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                string queryStatement = DataAccess.GlabalSQLQueries.ReportDebtors;

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);

                    _con.Open();
                    _dap.Fill(DebtorTable);
                    _con.Close();
                }
            }
            DebtorTable.Columns.Add("4+ Months");
            DebtorTable.Columns.Add("3 Months");
            DebtorTable.Columns.Add("2 Months");
            DebtorTable.Columns.Add("1 Months");
            DebtorTable.Columns.Add("Current");
            DebtorTable.Columns.Add("Outstanding");
            DebtorTable.AcceptChanges();

            int CurrentMonthDate = DateTime.Now.Month;
            int CurrentYRDate = DateTime.Now.Year;

            //filling in Months owed
            foreach (DataRow row in DebtorTable.Rows)
            {
                int YearNumber = Convert.ToInt32(row["Transaction Date"].ToString().Substring(0, 4));
                int MonthNumber = Convert.ToInt32(row["Transaction Date"].ToString().Substring(5, 2));
                row["Outstanding"] = Convert.ToDouble(row["GoodsValueInAccountCurrency"]) - Convert.ToDouble(row["AllocatedValue"]);

                if (YearNumber != CurrentYRDate)
                {
                    row["4+ Months"] = row["GoodsValueInAccountCurrency"];
                }

                else if (MonthNumber == CurrentMonthDate && YearNumber == CurrentYRDate)
                {
                    row["Current"] = row["GoodsValueInAccountCurrency"];
                }

                else if ((MonthNumber - CurrentMonthDate) == -1 && YearNumber == CurrentYRDate)
                {
                    row["1 Months"] = row["GoodsValueInAccountCurrency"];
                }

                else if ((MonthNumber - CurrentMonthDate) == -2 && YearNumber == CurrentYRDate)
                {
                    row["2 Months"] = row["GoodsValueInAccountCurrency"];
                }

                else if ((MonthNumber - CurrentMonthDate) == -3 && YearNumber == CurrentYRDate)
                {
                    row["3 Months"] = row["GoodsValueInAccountCurrency"];
                }

                else if ((MonthNumber - CurrentMonthDate) <= -4 && YearNumber == CurrentYRDate)
                {
                    row["4+ Months"] = row["GoodsValueInAccountCurrency"];
                }
            }

            List<string> InterestingAccounts = new List<string>();
            DebtorTable.Columns.Add("Test");
            DebtorTable.AcceptChanges();
            foreach (DataRow row in DebtorTable.Rows)
            {

                if (row["4+ Months"].ToString() is not "" || row["3 Months"].ToString() is not "")
                {
                    row["Test"] = "Yes";
                    InterestingAccounts.Add(row["CustomerAccountNumber"].ToString());
                }

                if (row["2 Months"].ToString() is not "")
                {
                    double TwoMnthNumber = double.Parse((string)row["2 Months"]);
                    if (TwoMnthNumber < 0.00)
                    {
                        row["Test"] = "Yes Minus";
                        InterestingAccounts.Add(row["CustomerAccountNumber"].ToString());
                    }

                }

                if (row["1 Months"].ToString() is not "")
                {
                    double OneMnthNumber = double.Parse((string)row["1 Months"]);
                    if (OneMnthNumber < 0.00)
                    {
                        row["Test"] = "Yes Minus";
                        InterestingAccounts.Add(row["CustomerAccountNumber"].ToString());
                    }

                }

                if (row["Current"].ToString() is not "")
                {
                    double CurMnthNumber = double.Parse((string)row["Current"]);
                    if (CurMnthNumber < 0.00)
                    {
                        row["Test"] = "Yes Minus";
                        InterestingAccounts.Add(row["CustomerAccountNumber"].ToString());
                    }

                }
            }

            foreach (DataRow row in DebtorTable.Rows)
            {
                if (!InterestingAccounts.Contains(row["CustomerAccountNumber"].ToString()))
                {
                    row.Delete();
                }
            }
            DebtorTable.AcceptChanges();
            DataGrid1.ItemsSource = DebtorTable.DefaultView;
        }

        private void BtnReportDebtors_Click(object sender, RoutedEventArgs e)
        {
            SqlReportDebtorsList();
        }

        public void GetCSV()
        {
            var CurrentUser = Environment.UserName;
            string filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\Exports\\Debtors.xlsx";
            XLWorkbook wb = new XLWorkbook();

            DataView dv = (DataView)DataGrid1.ItemsSource;
            DataTable dt = dv.Table.Clone();
            foreach (DataRowView dataRowView in dv)
            {
                dt.ImportRow(dataRowView.Row);
            }
            var dataTableFromDataGrid = dt;

            DataTable exportDebtorList = new DataTable();

            //foreach(DataColumn col in dt.Columns)
            //{
            //    exportDebtorList.Columns.Add(col.ColumnName.ToString());
            //}
            //foreach(DataRow row in dt.Rows)
            //{

            //}


            //foreach(DataGridColumn col in DataGrid1.Columns)
            //{
            //    exportDebtorList.Columns.Add(col.Header.ToString());
            //}
            //foreach(DataRowView row in DataGrid1.ItemsSource)
            //{
            //    exportDebtorList.Rows.Add(row[0], row[1], row[2], row[3], row[4], row[5], row[6], row[7], row[8], row[9], row[10], row[11], row[12], row[13]);
            //}

            wb.Worksheets.Add(dataTableFromDataGrid, "Jobs");
            wb.SaveAs(filepath);


        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            GetCSV();
        }
    }
}
