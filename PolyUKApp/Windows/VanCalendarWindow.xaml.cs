using PolyUKApp.MVVM.View;
using PolyUKApp.SQL;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Globalization;
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
using static PolyUKApp.MVVM.View.VanCalendarPanel;
using System.IO;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.Reflection;
using PolyUKApp.SQL.Models;
using Mysqlx.Resultset;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace PolyUKApp.Windows
{
    /// <summary>
    /// Interaction logic for VanCalendarWindow.xaml
    /// </summary>
    public partial class VanCalendarWindow : Window
    {
        static DateTime currentDateTime = DateTime.Now;
        static int currentYear = currentDateTime.Year;
        static int currentMonth = currentDateTime.Month;

        //statics to pass to UC Calendar panel
        public static int static_month, static_year;

        public VanCalendarWindow()
        {
            InitializeComponent();
            CalendarDays();
            MySQLGetVan();
            UserButtonChecker();
            NotificationLight();
        }
        public void CalendarDays()
        {

            //Get first day of month
            DateTime StartofMonth = new DateTime(currentYear, currentMonth, 1);

            //Get count of days in month
            int days = DateTime.DaysInMonth(currentYear, currentMonth);

            //convert start month to int
            int dayOfWeek = Convert.ToInt32(StartofMonth.DayOfWeek.ToString("d")) + 1;

            //matching static values to current month and year
            static_month = currentMonth;
            static_year = currentYear;

            //Month Year name
            String currentMonthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(currentMonth);
            TextBlockMonth.Text = currentMonthName;
            TextBlockYear.Text = currentYear.ToString();
            switch (currentMonth)
            {
                case 1:
                    TextBlockMonthM2.Text = "11";
                    break;
                case 2:
                    TextBlockMonthM2.Text = "12";
                    break;
                default:
                    TextBlockMonthM2.Text = ((currentMonth) - 2).ToString();
                    break;
            }
            switch (currentMonth)
            {
                case 1:
                    TextBlockMonthM1.Text = "12";
                    break;
                default:
                    TextBlockMonthM1.Text = ((currentMonth) - 1).ToString();
                    break;
            }
            TextBlockMonthC.Text = currentMonth.ToString();
            switch (currentMonth)
            {
                case 12:
                    TextBlockMonthP1.Text = "1";
                    break;
                default:
                    TextBlockMonthP1.Text = ((currentMonth) + 1).ToString();
                    break;
            }
            switch (currentMonth)
            {
                case 11:
                    TextBlockMonthP2.Text = "1";
                    break;
                case 12:
                    TextBlockMonthP2.Text = "2";
                    break;
                default:
                    TextBlockMonthP2.Text = ((currentMonth) + 2).ToString();
                    break;
            }

            //Blank UC
            for (int i = 1; i < dayOfWeek; i++)
            {
                CalendarPanelBlank calendarPanelBlank = new CalendarPanelBlank();
                CalData.Children.Add(calendarPanelBlank);
            }

            //UC for Days
            for (int i = 1; i <= days; i++)
            {
                VanCalendarPanel calendarVanPanelDays = new VanCalendarPanel();
                calendarVanPanelDays.Days(i);
                CalData.Children.Add(calendarVanPanelDays);
            }
        }

        private void BtnLeftYR_Click(object sender, RoutedEventArgs e)
        {
            CalData.Children.Clear();
            currentYear--;
            CalendarDays();
        }

        private void BtnRightYR_Click(object sender, RoutedEventArgs e)
        {
            CalData.Children.Clear();
            currentYear++;
            CalendarDays();
        }

        private void BtnLeft_Click(object sender, RoutedEventArgs e)
        {
            //clear days
            CalData.Children.Clear();
            //increment month down 1
            if (currentMonth == 1)
            {
                currentMonth = 13;
                currentYear--;
            }
            currentMonth--;

            //re-run method
            CalendarDays();
        }

        private void BtnRight_Click(object sender, RoutedEventArgs e)
        {
            //clear days
            CalData.Children.Clear();
            //increment month up 1
            if (currentMonth == 12)
            {
                currentMonth = 0;
                currentYear++;
            }
            currentMonth++;
            //re-run method
            CalendarDays();
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

        private void TopBar0_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void VanDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var VisitCellInfo = VanDataGrid.SelectedCells[4];
            var IDName = (VisitCellInfo.Column.GetCellContent(VisitCellInfo.Item) as TextBlock).Text;
            System.Windows.Clipboard.Clear();
            System.Windows.Clipboard.SetDataObject(IDName.ToString());

            var ThisWindow = Window.GetWindow(this);

            double WindowLeft = ThisWindow.Left;
            double WindowTop = ThisWindow.Top;
            double WindowHeight = ThisWindow.Height;
            double WindowWidth = ThisWindow.Width;

            if (ThisWindow.WindowState == WindowState.Maximized)
            {
                var VisitInfoBox = new VanVisitInfoWindow();
                VisitInfoBox.WindowState = WindowState.Maximized;
                VisitInfoBox.Closed += childFormEditVisitClosed;
                VisitInfoBox.Show();
            }
            else
            {
                var VisitInfoBox = new VanVisitInfoWindow { Left = WindowLeft, Top = WindowTop, Width = WindowWidth, Height = WindowHeight };
                VisitInfoBox.Closed += childFormEditVisitClosed;
                VisitInfoBox.Show();
            }


        }

        private void BtnAddVisit_Click(object sender, RoutedEventArgs e)
        {
            var ThisWindow = Window.GetWindow(this);

            double WindowLeft = ThisWindow.Left;
            double WindowTop = ThisWindow.Top;
            double WindowHeight = ThisWindow.Height;
            double WindowWidth = ThisWindow.Width;

            if (ThisWindow.WindowState == WindowState.Maximized)
            {
                var VisitAddBox = new VanVisitAddWindow();
                VisitAddBox.WindowState = WindowState.Maximized;
                VisitAddBox.Closed += childFormAddVisitClosed;
                VisitAddBox.Show();
            }
            else
            {
                var VisitAddBox = new VanVisitAddWindow { Left = WindowLeft, Top = WindowTop, Width = WindowWidth, Height = WindowHeight };
                VisitAddBox.Closed += childFormAddVisitClosed;
                VisitAddBox.Show();
            }
        }

        private void BtnEditVisit_Click(object sender, RoutedEventArgs e)
        {
            var VisitCellInfo = VanDataGrid.SelectedCells[4];
            var IDName = (VisitCellInfo.Column.GetCellContent(VisitCellInfo.Item) as TextBlock).Text;
            System.Windows.Clipboard.Clear();
            System.Windows.Clipboard.SetDataObject(IDName.ToString());

            var ThisWindow = Window.GetWindow(this);

            double WindowLeft = ThisWindow.Left;
            double WindowTop = ThisWindow.Top;
            double WindowHeight = ThisWindow.Height;
            double WindowWidth = ThisWindow.Width;

            if (ThisWindow.WindowState == WindowState.Maximized)
            {
                var VisitInfoBox = new VanVisitInfoWindow();
                VisitInfoBox.WindowState = WindowState.Maximized;
                VisitInfoBox.Closed += childFormEditVisitClosed;
                VisitInfoBox.Show();
                
            }
            else
            {
                var VisitInfoBox = new VanVisitInfoWindow { Left = WindowLeft, Top = WindowTop, Width = WindowWidth, Height = WindowHeight };
                VisitInfoBox.Closed += childFormEditVisitClosed;
                VisitInfoBox.Show();
                
            }
            BtnEditVisit.Visibility = Visibility.Hidden;
            BtnDeleteVisit.Visibility = Visibility.Hidden;
            BtnCOmpleteVisit.Visibility = Visibility.Hidden;
        }
        public void childFormEditVisitClosed(object sender, EventArgs e)
        {
            ((VanVisitInfoWindow)sender).Closed -= childFormEditVisitClosed;
            MySQLGetVan();
            CalData.Children.Clear();
            CalendarDays();

        }
        void childFormAddVisitClosed(object sender, EventArgs e)
        {
            ((VanVisitAddWindow)sender).Closed -= childFormAddVisitClosed;
            MySQLGetVan();
            CalData.Children.Clear();
            CalendarDays();
            
        }

        private void VanDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var VisitCellInfo = VanDataGrid.SelectedCells[4];
            
            if (VisitCellInfo.Column != null)
            {
                BtnEditVisit.Visibility = Visibility.Visible;
                BtnDeleteVisit.Visibility = Visibility.Visible;
                BtnCOmpleteVisit.Visibility = Visibility.Visible;
            }
            else
            {
                BtnEditVisit.Visibility = Visibility.Collapsed;
                BtnDeleteVisit.Visibility = Visibility.Collapsed;
                BtnCOmpleteVisit.Visibility = Visibility.Collapsed;
            }


        }

        private void BtnDeleteVisit_Click(object sender, RoutedEventArgs e)
        {
            var VisitCellInfo = VanDataGrid.SelectedCells[4];
            var IDName = (VisitCellInfo.Column.GetCellContent(VisitCellInfo.Item) as TextBlock).Text;

            DialogResult dialogResult = (System.Windows.Forms.MessageBox.Show("Are you sure?", "Delete Entry", MessageBoxButtons.YesNo));
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                var ConnectionString = DataAccess.GlobalSQL.ConnectionMySQLVan;
                using (MySqlConnection _con = new MySqlConnection(ConnectionString))
                {
                    var CommandStatement = DataAccess.GlobalSQLNonQueries.DeleteFromVanList;
                    using (MySqlCommand _cmd = new MySqlCommand(CommandStatement, _con))
                    {

                        _con.Open();
                        _cmd.Parameters.AddWithValue("@IDTEXT", IDName);
                        _cmd.ExecuteNonQuery();
                        _con.Close();
                    }
                }
                System.Windows.MessageBox.Show("Visit Deleted");
                MySQLGetVan();
                CalData.Children.Clear();
                CalendarDays();
            }
            else
            {
                System.Windows.MessageBox.Show("Action Cancelled");
            }

        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            MySQLGetVan();
            CalData.Children.Clear();
            CalendarDays();
        }

        private void BtnCSVExport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnviewlOldJobs_Click(object sender, RoutedEventArgs e)
        {
            var ThisWindow = Window.GetWindow(this);

            double WindowLeft = ThisWindow.Left;
            double WindowTop = ThisWindow.Top;
            double WindowHeight = ThisWindow.Height;
            double WindowWidth = ThisWindow.Width;

            if (ThisWindow.WindowState == WindowState.Maximized)
            {
                var OldJobsBox = new VanVisitListWindow();
                OldJobsBox.WindowState = WindowState.Maximized;
                OldJobsBox.Closed += childFormVisitListClosed;
                OldJobsBox.Show();
            }
            else
            {
                var OldJobsBox = new VanVisitListWindow { Left = WindowLeft, Top = WindowTop, Width = WindowWidth, Height = WindowHeight };
                OldJobsBox.Closed += childFormVisitListClosed;
                OldJobsBox.Show();
            }
        }
        void childFormVisitListClosed(object sender, EventArgs e)
        {
            ((VanVisitListWindow)sender).Closed -= childFormVisitListClosed;
            MySQLGetVan();
            CalData.Children.Clear();
            CalendarDays();

        }

        private void BtnCOmpleteVisit_Click(object sender, RoutedEventArgs e)
        {
            var VisitCellInfo = VanDataGrid.SelectedCells[4];
            var IDName = (VisitCellInfo.Column.GetCellContent(VisitCellInfo.Item) as TextBlock).Text;

            DialogResult dialogResult = (System.Windows.Forms.MessageBox.Show("Are you sure?", "Complete Entry", MessageBoxButtons.YesNo));
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                var ConnectionString = DataAccess.GlobalSQL.ConnectionMySQLVan;
                using (MySqlConnection _con = new MySqlConnection(ConnectionString))
                {
                    var CommandStatement = DataAccess.GlobalSQLNonQueries.CompleteFromVanList;
                    using (MySqlCommand _cmd = new MySqlCommand(CommandStatement, _con))
                    {

                        _con.Open();
                        _cmd.Parameters.AddWithValue("@IDTEXT", IDName);
                        _cmd.ExecuteNonQuery();
                        _con.Close();
                    }
                }
                System.Windows.MessageBox.Show("Visit Completed");
                MySQLGetVan();
                CalData.Children.Clear();
                CalendarDays();

            }
            else
            {
                System.Windows.MessageBox.Show("Action Cancelled");
            }

        }

        private void BtnViewPending_Click(object sender, RoutedEventArgs e)
        {
            MySQLGetPending();
            
        }

        private void BtnViewVisits_Click(object sender, RoutedEventArgs e)
        {
            MySQLGetVan();
            
        }

        public void MySQLGetVan()
        {
            var ConnectionString = DataAccess.GlobalSQL.ConnectionMySQLVan;
            DataTable VanList = new DataTable();

            using (MySqlConnection _con = new MySqlConnection(ConnectionString))
            {
                var QueryStatement = DataAccess.GlabalSQLQueries.VanList;
                using (MySqlCommand _cmd = new MySqlCommand(QueryStatement, _con))
                {
                    MySqlDataAdapter _dap = new MySqlDataAdapter(_cmd);
                    _con.Open();
                    _dap.Fill(VanList);
                    _con.Close();
                }
            }
            
            VanDataGridPending.ItemsSource = null;
            DataGridBorderPending.IsHitTestVisible = false;
            VanDataGridPending.IsHitTestVisible = false;
            
            VanDataGrid.ItemsSource = null;
            VanDataGrid.IsHitTestVisible = true;
            DataGridBorder.IsHitTestVisible = true;
            VanDataGrid.ItemsSource = VanList.DefaultView;

            BorderButtonsBottom.Visibility = Visibility.Visible;
            BorderPendingButtons.Visibility = Visibility.Collapsed;

            TxtGridName.Text = "Visit Details";
            NotificationLight();

        }

        private void VanDataGridPending_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var VisitCellInfo = VanDataGridPending.SelectedCells[4];
            var IDName = (VisitCellInfo.Column.GetCellContent(VisitCellInfo.Item) as TextBlock).Text;
            System.Windows.Clipboard.Clear();
            System.Windows.Clipboard.SetDataObject(IDName.ToString());

            var ThisWindow = Window.GetWindow(this);

            double WindowLeft = ThisWindow.Left;
            double WindowTop = ThisWindow.Top;
            double WindowHeight = ThisWindow.Height;
            double WindowWidth = ThisWindow.Width;

            if (ThisWindow.WindowState == WindowState.Maximized)
            {
                var VisitAddBox = new VanEditRequestVisit();
                VisitAddBox.WindowState = WindowState.Maximized;
                VisitAddBox.Closed += childFormEditRequestVisitClosed;
                VisitAddBox.Show();
            }
            else
            {
                var VisitAddBox = new VanEditRequestVisit { Left = WindowLeft, Top = WindowTop, Width = WindowWidth, Height = WindowHeight };
                VisitAddBox.Closed += childFormEditRequestVisitClosed;
                VisitAddBox.Show();
            }
        }

        private void VanDataGridPending_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var VisitCellInfo = VanDataGridPending.SelectedCells[4];

            if (VisitCellInfo.Column != null)
            {
                BtnEditRequest.Visibility = Visibility.Visible;
                BtnDeleteRequest.Visibility = Visibility.Visible;

            }
            else
            {
                BtnEditRequest.Visibility = Visibility.Collapsed;
                BtnDeleteRequest.Visibility = Visibility.Collapsed;

            }
        }

        public void MySQLGetPending()
        {
            var ConnectionString = DataAccess.GlobalSQL.ConnectionMySQLVan;
            DataTable VanList = new DataTable();

            using (MySqlConnection _con = new MySqlConnection(ConnectionString))
            {
                var QueryStatement = DataAccess.GlabalSQLQueries.VanListPendingSmall;
                using (MySqlCommand _cmd = new MySqlCommand(QueryStatement, _con))
                {
                    MySqlDataAdapter _dap = new MySqlDataAdapter(_cmd);
                    _con.Open();
                    _dap.Fill(VanList);
                    _con.Close();
                }
            }
            VanDataGrid.ItemsSource = null;
            VanDataGrid.IsHitTestVisible = false;
            DataGridBorder.IsHitTestVisible = false;

            VanDataGridPending.ItemsSource = null;
            VanDataGridPending.IsHitTestVisible = true;
            DataGridBorderPending.IsHitTestVisible = true;
            VanDataGridPending.ItemsSource = VanList.DefaultView;

            BorderButtonsBottom.Visibility = Visibility.Collapsed;
            BorderPendingButtons.Visibility = Visibility.Visible;

            TxtGridName.Text = "Pending Visit Requests";
            NotificationLight();

        }

        private void BtnAddRequest_Click(object sender, RoutedEventArgs e)
        {
            var ThisWindow = Window.GetWindow(this);

            double WindowLeft = ThisWindow.Left;
            double WindowTop = ThisWindow.Top;
            double WindowHeight = ThisWindow.Height;
            double WindowWidth = ThisWindow.Width;

            if (ThisWindow.WindowState == WindowState.Maximized)
            {
                var VisitAddBox = new VanRequestVisit();
                VisitAddBox.WindowState = WindowState.Maximized;
                VisitAddBox.Closed += childFormRequestVisitClosed;
                VisitAddBox.Show();
            }
            else
            {
                var VisitAddBox = new VanRequestVisit { Left = WindowLeft, Top = WindowTop, Width = WindowWidth, Height = WindowHeight };
                VisitAddBox.Closed += childFormRequestVisitClosed;
                VisitAddBox.Show();
            }
        }
        void childFormRequestVisitClosed(object sender, EventArgs e)
        {
            ((VanRequestVisit)sender).Closed -= childFormRequestVisitClosed;
            MySQLGetPending();

        }

        private void BtnEditRequest_Click(object sender, RoutedEventArgs e)
        {
            var VisitCellInfo = VanDataGridPending.SelectedCells[4];
            var IDName = (VisitCellInfo.Column.GetCellContent(VisitCellInfo.Item) as TextBlock).Text;
            System.Windows.Clipboard.Clear();
            System.Windows.Clipboard.SetDataObject(IDName.ToString());

            var ThisWindow = Window.GetWindow(this);

            double WindowLeft = ThisWindow.Left;
            double WindowTop = ThisWindow.Top;
            double WindowHeight = ThisWindow.Height;
            double WindowWidth = ThisWindow.Width;

            if (ThisWindow.WindowState == WindowState.Maximized)
            {
                var VisitAddBox = new VanEditRequestVisit();
                VisitAddBox.WindowState = WindowState.Maximized;
                VisitAddBox.Closed += childFormEditRequestVisitClosed;
                VisitAddBox.Show();
            }
            else
            {
                var VisitAddBox = new VanEditRequestVisit { Left = WindowLeft, Top = WindowTop, Width = WindowWidth, Height = WindowHeight };
                VisitAddBox.Closed += childFormEditRequestVisitClosed;
                VisitAddBox.Show();
            }
        }
        void childFormEditRequestVisitClosed(object sender, EventArgs e)
        {
            ((VanEditRequestVisit)sender).Closed -= childFormEditRequestVisitClosed;
            MySQLGetPending();
            CalData.Children.Clear();
            CalendarDays();

        }

        private void BtnDeleteRequest_Click(object sender, RoutedEventArgs e)
        {
            var VisitCellInfo = VanDataGridPending.SelectedCells[4];
            var IDName = (VisitCellInfo.Column.GetCellContent(VisitCellInfo.Item) as TextBlock).Text;

            DialogResult dialogResult = (System.Windows.Forms.MessageBox.Show("Are you sure?", "Delete Entry", MessageBoxButtons.YesNo));
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                var ConnectionString = DataAccess.GlobalSQL.ConnectionMySQLVan;
                using (MySqlConnection _con = new MySqlConnection(ConnectionString))
                {
                    var CommandStatement = DataAccess.GlobalSQLNonQueries.DeleteFromVanPendingList;
                    using (MySqlCommand _cmd = new MySqlCommand(CommandStatement, _con))
                    {

                        _con.Open();
                        _cmd.Parameters.AddWithValue("@IDTEXT", IDName);
                        _cmd.ExecuteNonQuery();
                        _con.Close();
                    }
                }
                System.Windows.MessageBox.Show("Visit Deleted");
                MySQLGetPending();
            }
            else
            {
                System.Windows.MessageBox.Show("Action Cancelled");
            }
        }

        public void UserButtonChecker()
        {
            string loginname = Environment.UserName;
            if (loginname == "MatthewKavanagh" || loginname == "JakeBassi" || loginname == "SophieGroth" || loginname == "AntonyGroth")
            {
                StackPanelButtonsBottom.Visibility = Visibility.Visible;
            }

            else
            {
                StackPanelButtonsBottom.Visibility = Visibility.Hidden;
            }
        }

        private void BtnMap_Click(object sender, RoutedEventArgs e)
        {
            Process myProcess = new Process();
            try
            {
                // true is the default, but it is important not to set it to false
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = "https://www.google.com/maps/d/u/0/edit?mid=1xu_QXJRraGdHnCEDXrK7NctwMyFVqHE&ll=52.36744807428512%2C-3.4843077500000064&z=7";
                myProcess.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void BtnMapGOOG_Click(object sender, RoutedEventArgs e)
        {

            var ThisWindow = Window.GetWindow(this);

            double WindowLeft = ThisWindow.Left;
            double WindowTop = ThisWindow.Top;
            double WindowHeight = ThisWindow.Height;
            double WindowWidth = ThisWindow.Width;

            if (ThisWindow.WindowState == WindowState.Maximized)
            {
                var VisitAddBox = new VanMapWindow();
                VisitAddBox.WindowState = WindowState.Maximized;
                VisitAddBox.Show();
            }
            else
            {
                var VisitAddBox = new VanMapWindow { Left = WindowLeft, Top = WindowTop, Width = WindowWidth, Height = WindowHeight };
                VisitAddBox.Show();
            }
        }
    

        public void NotificationLight()
        {
            var ConnectionString = DataAccess.GlobalSQL.ConnectionMySQLVan;
            DataTable VanList = new DataTable();

            using (MySqlConnection _con = new MySqlConnection(ConnectionString))
            {
                var QueryStatement = DataAccess.GlabalSQLQueries.VanListPendingSmall;
                using (MySqlCommand _cmd = new MySqlCommand(QueryStatement, _con))
                {
                    MySqlDataAdapter _dap = new MySqlDataAdapter(_cmd);
                    _con.Open();
                    _dap.Fill(VanList);
                    _con.Close();
                }
            }
            if (VanList.Rows.Count > 0)
            {
                string notificationNnumber = VanList.Rows.Count.ToString();
                NotificationNumber.Text = notificationNnumber;
                RectangleNotification.Visibility = Visibility.Visible;
                NotificationNumber.Visibility = Visibility.Visible;
            }
            else
            {
                RectangleNotification.Visibility = Visibility.Hidden;
                NotificationNumber.Visibility = Visibility.Hidden;

            }
        }

        /*public void GetVanExcel()
        {
            string CurrentUser = Globals.Username;
            string filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Waste Collection\\2024 Collection List Database.xlsx;";
            string conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + @"Extended Properties='Excel 8.0;HDR=Yes;'";
            DataTable VanList = new DataTable();

            using (OleDbConnection _con = new OleDbConnection(conn))
            {

                using (OleDbCommand _cmd = new OleDbCommand(DataAccess.GlabalSQLQueries.VanListDisplay, _con))
                {
                    OleDbDataAdapter _dap = new OleDbDataAdapter(_cmd);

                    _con.Open();
                    _dap.Fill(VanList);
                    _con.Close();

                }
                DataTable SmallVanList = new DataTable();
                SmallVanList.Columns.Add("Company");
                SmallVanList.Columns.Add("Postcode");
                SmallVanList.Columns.Add("Visit Type");
                SmallVanList.Columns.Add("ID");

                foreach (DataRow Row in VanList.Rows)
                {
                    SmallVanList.Rows.Add(Row["Company Name"] + "    ", Row["Postcode"] + "     ", Row["Visit Type"] + "   ", Row["ID"]);
                }
                VanDataGrid.ItemsSource = null;
                VanDataGrid.ItemsSource = SmallVanList.DefaultView;
            }
        }*/

        /*public void GetCSV()
        {
            string CurrentUser = Globals.Username;
            string filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Waste Collection\\2024 Collection List Database.csv";
            DataTable CSVList = new DataTable();
            foreach(var headerline in File.ReadLines(filepath).Take(1))
            {
                foreach (var headerItem in headerline.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    CSVList.Columns.Add(headerItem.Trim().Replace("\"", ""));
                }
            }
            foreach (var line in File.ReadLines(filepath).Skip(1))
            {
                CSVList.Rows.Add(line.Replace("\"", "").Split(','));
            }
            VanDataGrid.ItemsSource = null;
            VanDataGrid.ItemsSource = CSVList.DefaultView;
        */

    }
}
