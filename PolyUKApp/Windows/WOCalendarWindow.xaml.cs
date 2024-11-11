using Microsoft.Data.SqlClient;
using PolyUKApp.MVVM.View;
using PolyUKApp.SQL;
using PolyUKApp.Windows;
using System;
using System.Collections.Generic;
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

namespace PolyUKApp.Windows
{
    /// <summary>
    /// Interaction logic for WOCalendarWindow.xaml
    /// </summary>
    /// 

    static class DateTimeExtensions
    {
        static GregorianCalendar _gc = new GregorianCalendar();
        public static int GetWeekOfMonth(this DateTime time)
        {
            DateTime first = new DateTime(time.Year, time.Month, 1);
            return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
        }

        static int GetWeekOfYear(this DateTime time)
        {
            return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }
    }

    public partial class WOCalendarWindow : Window
    {
        static DateTime currentDateTime = DateTime.Now;
        static int currentYear = currentDateTime.Year;
        static int currentMonth = currentDateTime.Month;

        //statics to pass to UC Calendar panel
        public static int static_month, static_year;

        public WOCalendarWindow()
        {
            InitializeComponent();
            CalendarDays();
            DisplayCalendarList();
        }

        private void CalendarDays()
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
                CalendarPanel calendarPanelDays = new CalendarPanel();
                calendarPanelDays.Days(i);
                CalData.Children.Add(calendarPanelDays);
            }
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

        private void WODataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var WOCellInfo = WODataGrid.SelectedCells[0];
            var WONumber = (WOCellInfo.Column.GetCellContent(WOCellInfo.Item) as TextBlock).Text;
            System.Windows.Clipboard.Clear();
            System.Windows.Clipboard.SetText(WONumber.ToString());

            var ThisWindow = Window.GetWindow(this);

            double WindowLeft = ThisWindow.Left;
            double WindowTop = ThisWindow.Top;
            double WindowHeight = ThisWindow.Height;
            double WindowWidth = ThisWindow.Width;

            if (ThisWindow.WindowState == WindowState.Maximized)
            {
                var WOInfoBox = new WOInfoWindow();
                WOInfoBox.WindowState = WindowState.Maximized;
                WOInfoBox.Show();
            }
            else
            {
                var WOInfoBox = new WOInfoWindow { Left = WindowLeft, Top = WindowTop, Width = WindowWidth, Height = WindowHeight };
                WOInfoBox.Show();
            }
        }

        private void DisplayCalendarList()
        {
            string connectionString = DataAccess.GlobalSQL.Connection;
            DataTable WOListTable = new DataTable("WOListTable");
            DataTable WOInfoTable = new DataTable("WOInfoTable");

            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                _con.Open();
                String queryStatement = DataAccess.GlabalSQLQueries.WODetailsList;
                String queryStatement2 = DataAccess.GlabalSQLQueries.WOInfoForList;

                using(SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    _cmd.Parameters.AddWithValue("@WOStatus", "New");
                    _cmd.Parameters.AddWithValue("@WOStatus1", "Issued");
                    _cmd.Parameters.AddWithValue("@WOStatus2", "Allocated");
                    _dap.Fill(WOListTable);
                }
                using (SqlCommand _cmd2 = new SqlCommand(queryStatement2 , _con))
                {
                    SqlDataAdapter _dap2 = new SqlDataAdapter(_cmd2);
                    _cmd2.Parameters.AddWithValue("@Status", "New");
                    _cmd2.Parameters.AddWithValue("@Status1", "Issued");
                    _cmd2.Parameters.AddWithValue("@Status2", "Allocated");
                    _dap2.Fill(WOInfoTable);
                }

                WOListTable.PrimaryKey = new DataColumn[] {WOListTable.Columns[0]};
                WOInfoTable.PrimaryKey = new DataColumn[] {WOInfoTable.Columns[0]};

                WOListTable.Merge(WOInfoTable);

                DataTable WOCombinedListInfo = new DataTable();
                WOCombinedListInfo.Columns.Add("Order");
                WOCombinedListInfo.Columns.Add("Customer");
                WOCombinedListInfo.Columns.Add("Qty");
                WOCombinedListInfo.Columns.Add("Due Date");
                foreach(DataRow row in WOListTable.Rows)
                {
                    object CellTypeValue = row["WOType"];
                    if(CellTypeValue.ToString() == "INTERNAL")
                    {
                        WOCombinedListInfo.Rows.Add(row["WONumber"], "  " + row["CustomerAccountName"] + "  ", Convert.ToDouble(row["Quantity"]), row["PromisedDeliveryDate"]);
                    }
                }

                WODataGrid.ItemsSource = WOCombinedListInfo.DefaultView;
            }
        }
    }
}
