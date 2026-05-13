using ClosedXML.Excel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using PolyUKApp.MVVM.ViewModel;
using PolyUKApp.SQL;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace PolyUKApp.Windows
{
    /// <summary>
    /// Interaction logic for CallDataWindow.xaml
    /// </summary>
    public partial class CallDataWindow : Window
    {

        String connectionstring = DataAccess.GlobalSQL.Connection;
        String CurrentUser = Environment.UserName;
        DataTable ExcludeTable = new DataTable("ExcludeTable");
        DataTable DaysWorkedTable = new DataTable("DaysWorked");
        DataTable filteredDays = new DataTable("FilteredDays");
        DataTable SirusDataTable = new DataTable("SirusData");
        DataTable WeeklyCallTable = new DataTable("WeeklyCallTable");
        int CurrentWeekNum = ISOWeek.GetWeekOfYear(DateTime.Now);
        List<string> ExcludeList = new List<string>();

        private readonly MainViewModel _viewModel = new();


        public CallDataWindow()
        {

            InitializeComponent();
            DataContext = _viewModel;
            CreateDaysWorkedTable();
            CreateExcludeList();
            InitialiseCallGrid();
            CreateWeeklyCallTable();
            FillWeeklyTable();
            LoadChart(SirusDataTable, DaysWorkedTable);

        }

        private bool _isScrolling = false;

        public void LoadChart(DataTable sirusDataTable, DataTable daysWorkedTable)
        {
            _viewModel.LoadData(sirusDataTable, daysWorkedTable);
        }

        string FormatDuration(double totalSeconds)
        {
            TimeSpan ts = TimeSpan.FromSeconds(totalSeconds);
            return $"{(int)ts.TotalHours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
        }

        double GetDaysWorked(string owner)
        {
            DataRow match = filteredDays.AsEnumerable()
                .FirstOrDefault(row => row.Field<string>("SalesPerson").Equals(owner, StringComparison.OrdinalIgnoreCase));

            return match != null ? Convert.ToDouble(match["DaysWorked"]) : 5.0; // default to 5 if not found


        }

        double GetDaysWorkedLastWeek(string owner)
        {
            DataRow match = filteredDays.AsEnumerable()
                .FirstOrDefault(row => row.Field<string>("SalesPerson").Equals(owner, StringComparison.OrdinalIgnoreCase));

            return match != null ? Convert.ToDouble(match["DaysWorked"]) : 5.0; // default to 5 if not found
        }

        private void TopBar1_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CreateDaysWorkedTable()
        {
            String filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\data\\CallData\\DaysWorked.xlsx";
            OleDbConnection oleExcelConnection = default(OleDbConnection);

            var Connection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
            oleExcelConnection = new OleDbConnection(Connection);

            using (OleDbCommand _cmd = new OleDbCommand())
            {
                _cmd.Connection = oleExcelConnection;
                _cmd.CommandText = "SELECT * " +
                    "FROM [Days$] ";

                using (OleDbDataAdapter _dap = new OleDbDataAdapter())
                {
                    _dap.SelectCommand = _cmd;
                    _dap.Fill(DaysWorkedTable);
                }
            }

            string weekCol = $"W{CurrentWeekNum}";


            filteredDays.Columns.Add("SalesPerson", typeof(string));
            filteredDays.Columns.Add("DaysWorked", typeof(double));

            foreach (DataRow row in DaysWorkedTable.Rows)
            {
                filteredDays.Rows.Add(
                    row["SalesPerson"],
                    Convert.ToDouble(row[weekCol])
                );
            }

        }
        private void CreateExcludeList()
        {
            String filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\data\\CallData\\Excludes.xlsx";
            OleDbConnection oleExcelConnection = default(OleDbConnection);

            var Connection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
            oleExcelConnection = new OleDbConnection(Connection);

            using (OleDbCommand _cmd = new OleDbCommand())
            {
                _cmd.Connection = oleExcelConnection;
                _cmd.CommandText = "SELECT Excludes " +
                    "FROM [Excludes$] ";

                using (OleDbDataAdapter _dap = new OleDbDataAdapter())
                {
                    _dap.SelectCommand = _cmd;
                    _dap.Fill(ExcludeTable);
                }
            }

            foreach (DataRow row in ExcludeTable.Rows)
            {
                ExcludeList.Add(row["Excludes"].ToString());
            }
        }

        private void InitialiseCallGrid()
        {
            String filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\data\\CallData\\Sirus.xlsx";
            OleDbConnection oleExcelConnection = default(OleDbConnection);

            var Connection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
            oleExcelConnection = new OleDbConnection(Connection);

            using (OleDbCommand _cmd = new OleDbCommand())
            {
                _cmd.Connection = oleExcelConnection;
                _cmd.CommandText = "SELECT Extension, Owner, [Call Direction], [Call Time], Number, [Duration (s)] " +
                    "FROM [TMS - Itemised Extract$] " +
                    "WHERE [Duration (s)] <> 0 AND ([Call Direction] = 'Outbound' OR [Call Direction] = 'Inbound') " +
                    "ORDER BY Owner";

                using (OleDbDataAdapter _dap = new OleDbDataAdapter())
                {
                    _dap.SelectCommand = _cmd;
                    _dap.Fill(SirusDataTable);
                }
            }

            //Correct mobile numbers from +44 to 0
            foreach (DataRow row in SirusDataTable.Rows)
            {
                if (row["Number"].ToString().Substring(0, 3) == "+44")
                {
                    row["Number"] = "0" + row["Number"].ToString().Substring(3);
                }
            }

            //Add week number and remove excluded numbers
            SirusDataTable.Columns.Add("WeekNum");
            foreach (DataRow row in SirusDataTable.Rows)
            {
                if (ExcludeList.Contains(row["Number"]))
                {
                    row.Delete();
                }
                else
                {
                    row["WeekNum"] = ISOWeek.GetWeekOfYear(Convert.ToDateTime(row["Call Time"]));
                }
            }
            SirusDataTable.AcceptChanges();

            //Get Hour of call being made
            SirusDataTable.Columns.Add("CallHour");
            foreach (DataRow row in SirusDataTable.Rows)
            {
                row["CallHour"] = Convert.ToDateTime(row["Call Time"]).Hour;
            }
        }


        private void CreateWeeklyCallTable()
        {
            WeeklyCallTable = SirusDataTable.Copy();
            foreach (DataRow row in WeeklyCallTable.Rows)
            {
                if (Convert.ToInt32(row["WeekNum"]) != CurrentWeekNum)
                {
                    row.Delete();
                }
            }
            WeeklyCallTable.AcceptChanges();

            //Saving as Excel on update

            string filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\data\\CallData\\CurrentWeekTimes.xlsx";
            XLWorkbook wb = new XLWorkbook();

            var dataTableFromDataGrid = WeeklyCallTable;

            DataTable exportDebtorList = new DataTable();

            wb.Worksheets.Add(dataTableFromDataGrid, "ThisWeek");
            wb.SaveAs(filepath);

        }

        private void FillWeeklyTable()
        {
            //Defining Days to include
            var dayMap = new Dictionary<string, DayOfWeek>
                {
                    { "Monday", DayOfWeek.Monday },
                    { "Tuesday", DayOfWeek.Tuesday },
                    { "Wednesday", DayOfWeek.Wednesday },
                    { "Thursday", DayOfWeek.Thursday },
                    { "Friday", DayOfWeek.Friday }
                };

            var ownerTargets = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Donna Rivera", 9000 },  // 2.5h
                    { "Jason Mayhew",   14400 },  // 4 hours
                    { "James Scurr",   18000 },  // 5 hours
                    { "James Woollard",   10800 },  // 3 hours
                    { "Ryan King",   14400 },  // 4 hours
                    { "Jack Mungall",   10800 },  // 3 hours
                    { "Max Arnold",   18000 }  // 8 hours

                };


            DataTable summaryTable = new DataTable();

            //Adding Columns
            summaryTable.Columns.Add("SalesPerson", typeof(string));
            foreach (var day in dayMap.Keys)
            {
                summaryTable.Columns.Add(day, typeof(string));
            }

            summaryTable.Columns.Add("CallsConnected", typeof(int));
            summaryTable.Columns.Add("AverageCallTime");
            summaryTable.Columns.Add("TotalTime");
            summaryTable.Columns.Add("Target", typeof(string));
            summaryTable.Columns.Add("TotalStatus", typeof(string)); // "over", "under", or "none"


            var grouped = WeeklyCallTable.AsEnumerable()
                .GroupBy(row => row.Field<string>("Owner"));

            foreach (var ownerGroup in grouped)
            {
                DataRow newRow = summaryTable.NewRow();
                newRow["SalesPerson"] = ownerGroup.Key;

                foreach (var (label, dow) in dayMap)
                {
                    newRow[label] = ownerGroup
                        .Where(row => DateTime.Parse(row.Field<string>("Call Time")).DayOfWeek == dow)
                        .Sum(row => row.Field<double>("Duration (s)")).ToString();
                }

                // Target and comparison
                if (ownerTargets.TryGetValue(ownerGroup.Key, out double fullWeekTarget))
                {
                    if (CurrentWeekNum == ISOWeek.GetWeekOfYear(DateTime.Now))
                    {
                        double daysWorked = GetDaysWorked(ownerGroup.Key);
                        double adjustedTarget = fullWeekTarget * (daysWorked / 5.0);
                        newRow["Target"] = adjustedTarget;
                    }
                    else
                    {
                        double daysWorked = GetDaysWorked(ownerGroup.Key);
                        double adjustedTarget = fullWeekTarget * (daysWorked / 5.0);
                        newRow["Target"] = adjustedTarget;
                    }

                }
                else
                {
                    newRow["Target"] = 1;
                }
                newRow["TotalTime"] = dayMap.Keys.Sum(day => Convert.ToDouble(newRow[day]));
                newRow["CallsConnected"] = ownerGroup.Count();
                summaryTable.Rows.Add(newRow);
            }

            foreach (DataRow row in summaryTable.Rows)
            {
                if (Convert.ToDouble(row["TotalTime"]) >= Convert.ToDouble(row["Target"]))
                {
                    row["TotalStatus"] = "over";
                }
                else
                {
                    row["TotalStatus"] = "under";
                }
            }

            //Average Time column
            foreach (DataRow row in summaryTable.Rows)
            {
                row["AverageCallTime"] = Convert.ToDouble(row["TotalTime"]) / Convert.ToDouble(row["CallsConnected"]);

            }
            //Convert seconds to Hrs, Mins, Secs
            foreach (DataRow row in summaryTable.Rows)
            {
                var TimeConvertAv = TimeSpan.FromSeconds(Convert.ToDouble(row["AverageCallTime"]));
                row["AverageCallTime"] = TimeConvertAv.ToString(@"hh\:mm\:ss");
                var TimeConvertTarget = TimeSpan.FromSeconds(Convert.ToDouble(row["Target"]));
                row["Target"] = TimeConvertTarget.ToString(@"hh\:mm\:ss");
                var TimeConvert = TimeSpan.FromSeconds(Convert.ToDouble(row["TotalTime"]));
                row["TotalTime"] = TimeConvert.ToString(@"hh\:mm\:ss");
                var TimeConvertMon = TimeSpan.FromSeconds(Convert.ToDouble(row["Monday"]));
                row["Monday"] = TimeConvertMon.ToString(@"hh\:mm\:ss");
                var TimeConvertTues = TimeSpan.FromSeconds(Convert.ToDouble(row["Tuesday"]));
                row["Tuesday"] = TimeConvertTues.ToString(@"hh\:mm\:ss");
                var TimeConvertWed = TimeSpan.FromSeconds(Convert.ToDouble(row["Wednesday"]));
                row["Wednesday"] = TimeConvertWed.ToString(@"hh\:mm\:ss");
                var TimeConvertThu = TimeSpan.FromSeconds(Convert.ToDouble(row["Thursday"]));
                row["Thursday"] = TimeConvertThu.ToString(@"hh\:mm\:ss");
                var TimeConvertFri = TimeSpan.FromSeconds(Convert.ToDouble(row["Friday"]));
                row["Friday"] = TimeConvertFri.ToString(@"hh\:mm\:ss");
            }


            WeeklyCallDataGrid.ItemsSource = summaryTable.DefaultView;

        }

        private void BtnThisWeek_Click(object sender, RoutedEventArgs e)
        {
            CurrentWeekNum = ISOWeek.GetWeekOfYear(DateTime.Now);

            DaysWorkedTable.Clear();
            filteredDays.Clear();
            filteredDays.Columns.Clear();
            CreateDaysWorkedTable();
            CreateWeeklyCallTable();
            FillWeeklyTable();
        }

        private void BtnLastWeek_Click(object sender, RoutedEventArgs e)
        {
            CurrentWeekNum = ISOWeek.GetWeekOfYear(DateTime.Now) - 1;

            DaysWorkedTable.Clear();
            filteredDays.Clear();
            filteredDays.Columns.Clear();
            CreateDaysWorkedTable();
            CreateWeeklyCallTable();
            FillWeeklyTable();
        }

        private void BtnCallTimeToggle_Click(object sender, RoutedEventArgs e)
        {
            if (TxtSalesPeople.Visibility == Visibility.Visible)
            {
                NameStack.Visibility = Visibility.Collapsed;
                TxtSalesPeople.Visibility = Visibility.Collapsed;
            }
            else
            {
                NameStack.Visibility = Visibility.Visible;
                TxtSalesPeople.Visibility = Visibility.Visible;
            }
        }
    }
}
