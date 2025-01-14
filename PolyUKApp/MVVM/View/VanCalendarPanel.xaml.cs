using PolyUKApp.Windows;
using PolyUKApp.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static PolyUKApp.Windows.CallTimeWindow;
using MySql.Data.MySqlClient;
using System.Xml.Linq;
using Org.BouncyCastle.Asn1.Pkcs;

namespace PolyUKApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for VanCalendarPanel.xaml
    /// </summary>
    public partial class VanCalendarPanel : System.Windows.Controls.UserControl
    {

        public static string static_day;
        
        public VanCalendarPanel()
        {
            InitializeComponent();

        }
        public void Days(int numdays)
        {
            LabelDays.Content = numdays++;
            VisitDisplay();
            TodayPanel();
        }

        public void TodayPanel()
        {
            int PanelDay = Convert.ToInt32(LabelDays.Content.ToString());
            int CurrentDay = Convert.ToInt32(DateTime.Now.ToString().Substring(0, 2));
            int PanelMonth = Convert.ToInt32(VanCalendarWindow.static_month.ToString());
            int CurrentMonth = Convert.ToInt32(DateTime.Now.ToString().Substring(3, 2));
            int PanelYear = Convert.ToInt32(VanCalendarWindow.static_year.ToString());
            int CurrentYear = Convert.ToInt32(DateTime.Now.ToString().Substring(6, 4));
            if (PanelDay == CurrentDay && PanelMonth == CurrentMonth && PanelYear == CurrentYear)
            {
                MainBorder.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(211, 211, 211));
            }
        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MainBorder.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(211, 211, 211));
        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MainBorder.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            TodayPanel();
        }

        private void LabelDays_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void VisitDisplay()
        {
            //pull date from Calendar
            int static_day = Convert.ToInt32(LabelDays.Content.ToString());
            string DayString = static_day.ToString("00");
            string MonthString = VanCalendarWindow.static_month.ToString("00");
            int MonthInt = Convert.ToInt32(MonthString);
            string YearString = VanCalendarWindow.static_year.ToString();
            int YearInt = Convert.ToInt32(YearString);

            //Excel Date Format
            var CalendarFormat = DayString + "/" + MonthString + "/" + YearString;

            var connectionString = DataAccess.GlobalSQL.ConnectionMySQLVan;
            DataTable VisitDetailsFull = new DataTable();

            using (MySqlConnection _con = new MySqlConnection(connectionString)) 
            {
                _con.Open();
                string queryStatement1 = DataAccess.GlabalSQLQueries.VanListCombo;

                using (MySqlCommand _cmd = new MySqlCommand(queryStatement1, _con))
                {
                    MySqlDataAdapter _dap = new MySqlDataAdapter(_cmd);
                    _dap.Fill(VisitDetailsFull);
                }

                foreach (DataRow row in VisitDetailsFull.Rows)
                {
                    String VanCellDate = row["collection_date"].ToString();

                    if (VanCellDate == "")
                    {

                    }
                    else if (VanCellDate.Substring(0,10) == CalendarFormat.ToString())
                    {
                        
                        TextBlock newVisit = new TextBlock();
                        newVisit.Name = "TestBox1";
                        newVisit.Text = row["company_name"].ToString() + "\r" + row["visit_type"] + " - (" + row["id"] + ")";
                        newVisit.Margin = new Thickness(5,1,5,0);
                        newVisit.Padding = new Thickness(5, 1, 0, 1);
                        newVisit.FontSize = 12;
                        newVisit.FontFamily = new System.Windows.Media.FontFamily("Aptos");
                        newVisit.Foreground = System.Windows.Media.Brushes.White;
                        var CreditChecker = row["credit_checked"].ToString();

                        if (CreditChecker == "No")
                        {
                            newVisit.Background = System.Windows.Media.Brushes.IndianRed;
                        }
                        else
                        {
                            BrushConverter bc = new BrushConverter();
                            newVisit.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#007FFF");
                        }
                        
                        EventPanel.Children.Add(newVisit);
                        newVisit.MouseDown += CalendarEventClick;
                        
                        

                    }
                }
                
            }
        }

        void CalendarEventClick(object sender, MouseButtonEventArgs e)
        {
            //trimming to get ID number:
            object HitBox = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (HitBox != null)
            {
                String HitResult = ((System.Windows.Controls.TextBlock)((System.Windows.Media.PointHitTestResult)HitBox).VisualHit).Text.ToString();

                var VisitIDSplit = HitResult.Split(new[] { '(' }, StringSplitOptions.None);
                var VisitIDTrim = VisitIDSplit[1].ToString().Split(new[] { ')' }, StringSplitOptions.RemoveEmptyEntries);
                int VisitID = Convert.ToInt32(VisitIDTrim[0]);

                System.Windows.Clipboard.Clear();
                System.Windows.Clipboard.SetDataObject(VisitID.ToString());

                var ThisWindow = Window.GetWindow(this);

                double WindowLeft = ThisWindow.Left;
                double WindowTop = ThisWindow.Top;
                double WindowHeight = ThisWindow.Height;
                double WindowWidth = ThisWindow.Width;

                if (ThisWindow.WindowState == WindowState.Maximized)
                {
                    var VisitInfoBox = new VanVisitInfoWindow();
                    VisitInfoBox.WindowState = WindowState.Maximized;
                    VisitInfoBox.Show();

                }
                else
                {
                    var VisitInfoBox = new VanVisitInfoWindow { Left = WindowLeft, Top = WindowTop, Width = WindowWidth, Height = WindowHeight };
                    VisitInfoBox.Show();

                }

            }

        }


        public void VisitDisplayRefresh()
        {
            VisitDisplay();
        }
    }
}
