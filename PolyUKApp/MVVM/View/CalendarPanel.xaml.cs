using Microsoft.Data.SqlClient;
using PolyUKApp.SQL;
using PolyUKApp.Windows;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PolyUKApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for CalendarPanel.xaml
    /// </summary>
    public partial class CalendarPanel : System.Windows.Controls.UserControl
    {
        TextBlock newEvent = new TextBlock();
        List hitResultsList = new List();
        public System.Windows.DependencyObject VisualHit { get; }

        public static string static_day;
        public CalendarPanel()
        {
            InitializeComponent();
        }

        private void LabelDays_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void Days(int numdays)
        {
            LabelDays.Content = numdays++;
            WODisplay();
            TodayPanel();
        }

        public void TodayPanel()
        {
            var CurrentUser = Environment.UserName;
            var folderpath = "C:\\Users\\" + CurrentUser + "\\AppData\\Roaming\\Matt K Programs\\Poly UK App";
            var filepath = "C:\\Users\\" + CurrentUser + "\\AppData\\Roaming\\Matt K Programs\\Poly UK App\\Theme.txt";

            int PanelDay = Convert.ToInt32(LabelDays.Content.ToString());
            int CurrentDay = Convert.ToInt32(DateTime.Now.ToString().Substring(0, 2));
            int PanelMonth = Convert.ToInt32(WOCalendarWindow.static_month.ToString());
            int CurrentMonth = Convert.ToInt32(DateTime.Now.ToString().Substring(3, 2));
            int PanelYear = Convert.ToInt32(WOCalendarWindow.static_year.ToString());
            int CurrentYear = Convert.ToInt32(DateTime.Now.ToString().Substring(6, 4));
            if (PanelDay == CurrentDay && PanelMonth == CurrentMonth && PanelYear == CurrentYear)
            {
                if (!File.Exists(filepath))
                {
                    MainBorder.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(211, 211, 211));
                }
                else if (File.Exists(filepath))
                {
                    String themeSetting = File.ReadAllText(filepath).ToString();

                    if (themeSetting == "Light")
                    {
                        MainBorder.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(211, 211, 211));
                    }
                    if (themeSetting == "Dark")
                    {
                        MainBorder.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 68, 68));
                    }
                }
                return;
            }
        }

        /*public void panelDisplay()
        {
            //string of day, month, year "00" display as ISO Date
            int static_day = Convert.ToInt32(LabelDays.Content.ToString());
            string DayString = static_day.ToString("00");
            string MonthString = WOCalendarWindow.static_month.ToString("00");
            int MonthInt = Convert.ToInt32(MonthString);
            string YearString = WOCalendarWindow.static_year.ToString();
            int YearInt = Convert.ToInt32(YearString);

            object ISODate = YearString + MonthString + DayString;
            LabelEvent1.Content = ISODate;
            var StatusNew = "New";
            var StatusIssued = "Issued";
            var StatusAlloc = "Allocated";

            //cross ref with WO Table
            string connectionString = DataAccess.GlobalSQL.Connection;

            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                _con.Open();
                String queryStatement = DataAccess.GlabalSQLQueries.WOQuery;

                //DateTime dateEdit = new DateTime(YearInt, MonthInt, static_day, 11, 0, 0);
                //DateTime SageDateEdit = new DateTime("@StartDateShort")

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    _cmd.Parameters.AddWithValue("@StartDateShort", ISODate);
                    _cmd.Parameters.AddWithValue("@Status", StatusNew);
                    _cmd.Parameters.AddWithValue("@Status1", StatusIssued);
                    _cmd.Parameters.AddWithValue("@Status2", StatusAlloc);

                    SqlDataReader _dr = _cmd.ExecuteReader();
                    if (_dr.Read())
                    {
                        LabelEvent2.Content = _dr["WONumber"].ToString();
                        LabelEvent2.Visibility = Visibility.Visible;
                        EventBorder2.Visibility = Visibility.Visible;
                        LabelEvent3.Content = ISODate.ToString();
                        LabelEvent3.Visibility = Visibility.Visible;
                        EventBorder3.Visibility = Visibility.Visible;
                        _dr.DisposeAsync();
                        _cmd.Dispose();
                    }
                    else
                    {
                        _dr.DisposeAsync();
                        _cmd.Dispose();
                    }
                }
                _con.Close();

            }
        }*/

        public void WODisplay()
        {
            //pull date from Calendar
            int static_day = Convert.ToInt32(LabelDays.Content.ToString());
            string DayString = static_day.ToString("00");
            string MonthString = WOCalendarWindow.static_month.ToString("00");
            int MonthInt = Convert.ToInt32(MonthString);
            string YearString = WOCalendarWindow.static_year.ToString();
            int YearInt = Convert.ToInt32(YearString);

            //WOList Date format
            var WODateFormat = YearString + "-" + MonthString + "-" + DayString;
            //Sage Date Format
            var SageDateFormat = DayString + "/" + MonthString + "/" + YearString;


            //LabelEvent1.Content = WODateFormat;
            //LabelEvent1.Visibility = Visibility.Visible;
            //Load combined WO Data List

            string connectionString = DataAccess.GlobalSQL.Connection;
            DataTable WOTable = new DataTable("WOList");
            DataTable WODetails = new DataTable("WODetailsList");

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
                    WODateTable.Rows.Add(row["SiWorksOrderID"], row["WONumber"], row["StartDate"].ToString().Substring(0, 10), row["DueDate"].ToString().Substring(0, 10));
                }

                DataTable WODetailsTable = new DataTable("WODetailsTable");
                WODetails.DefaultView.ToTable(true, "WONumber");
                WODetailsTable.Columns.Add("WO UID");
                WODetailsTable.Columns.Add("WO Number");
                WODetailsTable.Columns.Add("Promised Date");
                WODetailsTable.Columns.Add("Job Location");
                WODetailsTable.Columns.Add("Company Name");
                foreach (DataRow row in WODetails.Rows)
                {
                    object cellvalue = row["PromisedDeliveryDate"];
                    if (cellvalue == DBNull.Value)
                    {
                        WODetailsTable.Rows.Add(row["SiWorksOrderID"], row["WONumber"], "", row["WOType"], row["CustomerAccountName"]);
                    }
                    else
                    {
                        WODetailsTable.Rows.Add(row["SiWorksOrderID"], row["WONumber"].ToString().Substring(0, 10), row["PromisedDeliveryDate"].ToString().Substring(0, 10), row["WOType"], row["CustomerAccountName"]);
                    }
                }
                DataTable UniqueWOTable = WODetailsTable.DefaultView.ToTable(true);

                WODateTable.PrimaryKey = new DataColumn[] { WODateTable.Columns[0] };
                UniqueWOTable.PrimaryKey = new DataColumn[] { UniqueWOTable.Columns[0] };

                WODateTable.Merge(UniqueWOTable);

                foreach (DataRow row in WODateTable.Rows)
                {
                    object WOCellDateStart = row["Start Date"];
                    object WOCellDateDue = row["Due Date"];
                    object WOLocation = row["Job Location"];

                    object WOStartDay = row["Start Date"].ToString().Substring(8, 2);
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
                        List<String> range = Enumerable.Range(1, daysint)
                            .Select(i => WOSysDateStart.AddDays(i).ToString().Substring(0, 10) + ".")
                            .ToList();
                        for (int i = 0; i < range.Count; i++)
                        {
                            if (range[i].Contains(SageDateFormat.ToString()))
                            {
                                TextBlock newEvent = new TextBlock();
                                newEvent.FontSize = 11;
                                newEvent.Text = "" + "\r";
                                newEvent.Padding = new Thickness(5, 1, 0, 1);
                                newEvent.Foreground = System.Windows.Media.Brushes.White;
                                BrushConverter bc = new BrushConverter();
                                newEvent.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#007FFF");
                                EventPanel.Children.Add(newEvent);
                            }
                        }
                    };
                    if (WOCellDateStart.ToString() == WODateFormat.ToString() && WOLocation.ToString() == "INTERNAL")
                    {
                        TextBlock newEvent = new TextBlock();
                        newEvent.FontSize = 12;
                        newEvent.FontFamily = new System.Windows.Media.FontFamily("Aptos");
                        newEvent.Text = "Start " + row["WO Number"] + "\r" + row["Company Name"];
                        newEvent.Padding = new Thickness(5,1,0,1);
                        newEvent.Foreground = System.Windows.Media.Brushes.White;
                        BrushConverter bc = new BrushConverter();
                        newEvent.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#007FFF");
                        EventPanel.Children.Add(newEvent);
                        newEvent.MouseDown += CalendarVisitEventClick;
                        


                    }
                    else if (WOCellDateDue.ToString() == WODateFormat.ToString() && WOLocation.ToString() == "INTERNAL")
                    {
                        System.Windows.Controls.TextBlock newEvent = new System.Windows.Controls.TextBlock();
                        newEvent.FontSize = 12;
                        newEvent.FontFamily = new System.Windows.Media.FontFamily("Aptos");
                        newEvent.Text = "End " + row["WO Number"] + "\r" + row["Company Name"];
                        newEvent.Padding = new Thickness(5, 1, 0, 1);
                        newEvent.Foreground = System.Windows.Media.Brushes.White;
                        BrushConverter bc = new BrushConverter();
                        newEvent.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#007FFF");
                        EventPanel.Children.Add(newEvent);
                        newEvent.MouseDown += CalendarVisitEventClick;
                    }
                }
                _con.Close();
            }
        }

        void CalendarVisitEventClick(object sender, MouseButtonEventArgs e)
        {

            //trimming to get ID number:
            
            object HitBox = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (HitBox != null)
            {
                String HitResult = ((System.Windows.Controls.TextBlock)((System.Windows.Media.PointHitTestResult)HitBox).VisualHit).Text.ToString();

                var WONumSplit = HitResult.Split(new[] { ' ' }, StringSplitOptions.None);
                var WONumTrim = WONumSplit[1].Split(new[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);
                var WONumFinal = WONumTrim[0].ToString();

                System.Windows.Clipboard.Clear();
                System.Windows.Clipboard.SetDataObject(WONumFinal);

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




        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var CurrentUser = Environment.UserName;
            var folderpath = "C:\\Users\\" + CurrentUser + "\\AppData\\Roaming\\Matt K Programs\\Poly UK App";
            var filepath = "C:\\Users\\" + CurrentUser + "\\AppData\\Roaming\\Matt K Programs\\Poly UK App\\Theme.txt";

            if (!File.Exists(filepath))
            {
                MainBorder.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(211, 211, 211));
            }
            else if (File.Exists(filepath))
            {
                String themeSetting = File.ReadAllText(filepath).ToString();

                if (themeSetting == "Light")
                {
                    MainBorder.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(211, 211, 211));
                }
                if (themeSetting == "Dark")
                {
                    MainBorder.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 68, 68));
                }
            }
            return;

        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var CurrentUser = Environment.UserName;
            var folderpath = "C:\\Users\\" + CurrentUser + "\\AppData\\Roaming\\Matt K Programs\\Poly UK App";
            var filepath = "C:\\Users\\" + CurrentUser + "\\AppData\\Roaming\\Matt K Programs\\Poly UK App\\Theme.txt";

            if (!File.Exists(filepath))
            {
                MainBorder.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(243, 243, 243));
            }
            else if (File.Exists(filepath))
            {
                String themeSetting = File.ReadAllText(filepath).ToString();

                if (themeSetting == "Light")
                {
                    MainBorder.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(243, 243, 243));
                }
                if (themeSetting == "Dark")
                {
                    MainBorder.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 45));
                }
            }
            TodayPanel();
            return;
        }



        /*private void displayEvent()
{
string connectionString = DataAccess.GlobalSQL.Connection;

using (SqlConnection _con = new SqlConnection(connectionString))
{
_con.Open();
String queryStatement = DataAccess.GlabalSQLQueries.WOQuery;
using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
{
  string DayString = LabelDays.Content.ToString();
  string YearString = WOCalendarWindow.static_year.ToString();
  switch (WOCalendarWindow.static_month)
  {
      case >= 10:
          string MonthStringL = WOCalendarWindow.static_month.ToString();
          var testdateL = (YearString) + (MonthStringL) + (DayString);
          _cmd.Parameters.AddWithValue("@StartDateShort", testdateL);
          LabelEvent2.Content = testdateL;
          break;
      case < 10:
          string MonthString = "0" + WOCalendarWindow.static_month.ToString();
          var testdate = (YearString) + (MonthString) + (DayString);
          _cmd.Parameters.AddWithValue("@StartDateShort", testdate);
          LabelEvent2.Content = testdate;
          break;
  }

  SqlDataReader _dr = _cmd.ExecuteReader();
  if (_dr.FieldCount >= 1)
  {
      LabelEvent2.Content = _dr.FieldCount;
      _dr.DisposeAsync();
      _cmd.Dispose();
  }
  else
  {
      LabelEvent2.Content = "null";
      _dr.DisposeAsync();
      _cmd.Dispose();
  }
}
_con.Close();

}
}*/
    }
}
