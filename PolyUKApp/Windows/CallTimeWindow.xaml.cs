using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Policy;
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
using static System.Windows.Forms.Design.AxImporter;
using MessageBox = System.Windows.MessageBox;


namespace PolyUKApp.Windows
{
    /// <summary>
    /// Interaction logic for CallTimeWindow.xaml
    /// </summary>
    public partial class CallTimeWindow : Window
    {
        public static string SessionID;

        public CallTimeWindow()
        {
            InitializeComponent();
            LoadTheme();
            LoadDaily();
            LoadWeekly();
            string currentTime = DateTime.Now.ToString();
            DateTimeText.Text = currentTime;
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

        public static class Globals
        {
            public static String Username = Environment.UserName;
        }
        public DataTable readCSV(string filepath)
        {
            var dt = new DataTable();
            foreach (var headerLine in File.ReadLines(filepath).Take(1))
            {
                foreach (var headerItem in headerLine.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    dt.Columns.Add(headerItem.Trim().Replace("\"", "")
                        .Replace("Description", "Name")
                        .Replace("Tot Tlk (Out)", "Total Talk (Out)")
                        .Replace("Avg Tlk (Out)", "Avg Talk (Out)")
                        .Replace("Tot Talk (In)", "Total Talk (In)")
                        .Replace("Avg Tlk (In)", "Avg Talk (In)")
                        .Replace("Tot Tlk", "Total Talk")
                        .Replace("Avg Tlk", "Avg Talk"));
                }
            }

            foreach (var line in File.ReadLines(filepath).Skip(1))
            {
                dt.Rows.Add(line.Replace("\"", "").Split(','));
            }
            return dt;
        }
        public void LoadDaily()
        {
            string CurrentUser = Globals.Username;
            try
            {
                using DataTable dt = readCSV("C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\612d239751dd5a85_-5362eb36_18b5c897a7f_10e5.csv");
                if (dt.Rows.Count > 0)
                {
                    DataGrid1.ItemsSource = null;
                    DataGrid1.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error");
            }
        }
        public class State
        {
            public bool on { get; set; }

            //another properties
        }
        public class StateItem
        {
            public string index { get; set; }
            public State Statistics { get; set; }
        }

        //Not working yet, need a re-think! Must be better than csv in the long run. Access nested fields?
        public void LoadDailyJSON()
            {
                string CurrentUser = Globals.Username;
                var jsonPath = @"C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\612d239751dd5a85_-5362eb36_18b5c897a7f_10e5.JSON";
            StateItem[] states = JObject.Parse(jsonPath).Properties().Select(i => new StateItem { index = i.Name, Statistics = i.Value.ToObject<State>()}).ToArray();
            MessageBox.Show("test");


        }
        public void LoadWeekly()
        {
            string CurrentUser = Globals.Username;
            try
            {
                using DataTable dt2 = readCSV("C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\612d239751dd5a85_-5362eb36_18b5c897a7f_2cbb.csv");
                if (dt2.Rows.Count > 0)
                {
                    DataGrid2.ItemsSource = null;
                    DataGrid2.ItemsSource = dt2.DefaultView;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error");
            }
        }

        private void TextBlockRefreshExplainer_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlockRefreshExplainer.Text = "These tables will refresh the call information at 20 minutes past the hour.";
        }

        private void BtnRefreshCallTime_Click(object sender, RoutedEventArgs e)
        {
            LoadDaily();
            LoadWeekly();
            string currentTime = DateTime.Now.ToString();
            DateTimeText.Text = currentTime;
        }

        private void BtnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void BtnQueryServer_Click(object sender, RoutedEventArgs e)
        {
            string CurrentUser = Globals.Username;
            String filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\data\\CallTimeUpdate.txt";
            var testing = File.ReadAllText(filepath).ToString();

            int FileDay = Convert.ToInt32(testing.Substring(0, 2));
            int FileMonth = Convert.ToInt32(testing.Substring(3, 2));
            var FileYear = Convert.ToInt32(testing.Substring(6, 4));
            var FileHour = Convert.ToInt32(testing.Substring(11, 2));
            var FileMin = Convert.ToInt32(testing.Substring(14, 2));
            var FileSec = Convert.ToInt32(testing.Substring(17, 2));
            var FileTime = new DateTime(FileYear, FileMonth, FileDay, FileHour, FileMin, FileSec);
            var LastUpdateTime = FileTime.AddMinutes(-10);
            var TimeRange = Convert.ToInt32(DateTime.Now.ToString().Substring(14, 2));
            var modifiedtime = File.GetLastWriteTime(filepath);

            if (FileTime > DateTime.Now)
            {
                TextBlockRefreshExplainer.Text = "Updated within last ten minutes (" + LastUpdateTime + "), please wait...";
            }
            else if (TimeRange > 10 && TimeRange < 25)
            {
                TextBlockRefreshExplainer.Text = "Will auto update shortly (at 20min past hour), please wait...";
            }
            else
            {
                TextBlockRefreshExplainer.Text = "Downloading, please wait...";
                await Task.Delay(1000);
                AkixiPuller();
                DateTime currentTime = DateTime.Now;
                var AddedTime = currentTime.AddMinutes(10);
                File.WriteAllText(filepath, AddedTime.ToString());
                TextBlockRefreshExplainer.Text = "Complete";
                LoadDaily();
            }
        }

        private void BtnLight_Click(object sender, RoutedEventArgs e)
        {
            var CurrentUser = Environment.UserName;
            var filepath = "C:\\Users\\" + CurrentUser + "\\AppData\\Roaming\\Matt K Programs\\Poly UK App\\Theme.txt";
            AppTheme.ChangeTheme(new Uri("Theme/AppLight.xaml", UriKind.Relative));
            File.WriteAllText(filepath, "Light");
        }

        private void BtnDark_Click(object sender, RoutedEventArgs e)
        {
            var CurrentUser = Environment.UserName;
            var filepath = "C:\\Users\\" + CurrentUser + "\\AppData\\Roaming\\Matt K Programs\\Poly UK App\\Theme.txt";
            AppTheme.ChangeTheme(new Uri("Theme/AppDark.xaml", UriKind.Relative));
            File.WriteAllText(filepath, "Dark");

        }

        private void LoadTheme()
        {
            var CurrentUser = Environment.UserName;
            var folderpath = "C:\\Users\\" + CurrentUser + "\\AppData\\Roaming\\Matt K Programs\\Poly UK App";
            var filepath = "C:\\Users\\" + CurrentUser + "\\AppData\\Roaming\\Matt K Programs\\Poly UK App\\Theme.txt";


            if (!File.Exists(filepath))
            {
                Directory.CreateDirectory(folderpath);
                File.WriteAllText(filepath, "Light");
            }
            else if (File.Exists(filepath))
            {
                String themeSetting = File.ReadAllText(filepath).ToString();

                if (themeSetting == "Light")
                {
                    AppTheme.ChangeTheme(new Uri("Theme/AppLight.xaml", UriKind.Relative));
                }
                if (themeSetting == "Dark")
                {
                    AppTheme.ChangeTheme(new Uri("Theme/AppDark.xaml", UriKind.Relative));
                }
            }
            return;

        }

        


        // AKIXI UPDATER

        public void AkixiPuller()
        {
            string url = "https://horizon.akixi.com/CCS/API/v1";
            bool sessionAuth = false;
            string b64 = "bWF0dGhld2thdmFuYWdoQHBvbHl0aGVuZXVrLmNvLnVrOlAwbHlNSzEh";
            string ReportID = "612d239751dd5a85:-5362eb36:18b5c897a7f:10e5";

            var tenantId = url;
            var sessionClient = new RestClient(url);

            var sessionRequest = new RestRequest($"{tenantId}/session", Method.Post);
            sessionRequest.AddHeader("Content-Type", "application/json");
            sessionRequest.AddHeader("Accept", "application/json");
            RestResponse response = sessionClient.Execute(sessionRequest);
            var content = response.Content;
            var token = JObject.Parse(content)["SessionID"];
            SessionID = "JSESSIONID=" + token.ToString().Trim();
            if (SessionID.Length > 5)
            {
                var accessRequest = new RestRequest($"{tenantId}/login?&locale=en_GB", Method.Get);
                accessRequest.AddHeader("Authorization", "Basic " + b64);
                accessRequest.AddHeader("Cookie", SessionID);
                RestResponse accessResponse = sessionClient.Execute(accessRequest);
                HttpStatusCode statusCode = accessResponse.StatusCode;

                if (statusCode == HttpStatusCode.OK)  //
                {
                    sessionAuth = true;

                }
                else
                {
                    sessionAuth = false;

                }
            }
            else
            {
                //
            }
            if (sessionAuth)
            {
                string CurrentUser = Globals.Username;
                String filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\";
                string urlGR = "https://horizon.akixi.com/CCS/API/v1";
                var tenantIdGR = urlGR;
                var sessionClientGR = new RestClient(urlGR);
                var getReportRequest = new RestRequest();
                string jsonReply = "";
                int i = 1;
                string reportStatus = "WAITING"; //"ExecutionStatus": "WAITING"
                while ((reportStatus == "WAITING") && (i <= 5))
                {

                    getReportRequest = new RestRequest($"{tenantId}/report/" + ReportID + "/exec", Method.Get);
                    getReportRequest.AddHeader("Cookie", SessionID);
                    RestResponse getReportResponse = sessionClient.Execute(getReportRequest);
                    HttpStatusCode getReportstatusCode = getReportResponse.StatusCode;
                    var getReportcontent = getReportResponse.Content;
                    jsonReply = getReportcontent.ToString();
                    JObject reportReplyStatus = JObject.Parse(getReportResponse.Content);
                    reportStatus = (string)reportReplyStatus.SelectToken("ExecutionStatus");
                    i++;
                    Thread.Sleep(5000);
                }
                WebClient CSVclient = new WebClient();
                CSVclient.Headers.Add(HttpRequestHeader.Cookie, SessionID);
                CSVclient.DownloadFile("https://horizon.akixi.com/CCS/App/Horizon?ServletCmd=CMD_RPT_EXEC&Action=ExportCSV&ResType=HTML&ID=" + ReportID + "&RptPortalID=612d239751dd5a85%3A-5362eb36", filepath + "\\" + ReportID.Replace(":", "_") + ".csv");
                var logoutRequest = new RestRequest($"{tenantId}/logout", Method.Get);
                logoutRequest.AddHeader("Cookie", SessionID);
                RestResponse logoutresponse = sessionClient.Execute(logoutRequest);
            }
            else
            {
                //
            }
        }
    }

        
    
}
