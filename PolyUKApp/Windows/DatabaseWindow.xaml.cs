using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.ApplicationServices;
using PolyUKApp.SQL;
using PolyUKApp.SQL.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
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

        private void SqlCommsCRM()
        {
            string connectionString = DataAccess.GlobalSQL.ConnectionCRM;
            DataTable commsTable = new DataTable();

            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                var commandStatement = DataAccess.GlobalSQLNonQueries.WriteCRMComms;
                //var queryStatement = DataAccess.GlabalSQLQueries.ReadCRMComms;
                //_con.Open();

                using (SqlCommand _cmd =  new SqlCommand(commandStatement, _con))
                {
                    _con.Open();
                    //SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    //_dap.Fill(commsTable);
                    _cmd.ExecuteNonQuery();
                    _con.Close();
                }
                System.Windows.MessageBox.Show("Done");
            }
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
    }
}
