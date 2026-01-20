using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for WasteTransferNoteWindow.xaml
    /// </summary>
    public partial class WasteTransferNoteWindow : Window
    {

        String CurrentUser = Environment.UserName;

        public WasteTransferNoteWindow()
        {
            InitializeComponent();
            InfoGetAppData();
        }

        private void TopBar1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
        private void TopBar2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public DataTable readCSV(string filepath)

        {
            var CouncilListCode = new DataTable();
            foreach (var headerLine in File.ReadLines(filepath).Take(1))
            {
                foreach (var headerItem in headerLine.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    CouncilListCode.Columns.Add(headerItem.Trim().Replace("\"", "")
                        .Replace("pcd", "Postcode")
                        .Replace("lad23cd", "Council Code")
                        .Replace("lad23nm", "Council Name"));

                }
            }

            foreach (var line in File.ReadLines(filepath).Skip(1))
            {
                CouncilListCode.Rows.Add(line.Replace("\"", "").Split(','));
            }
            return CouncilListCode;
        }

        public DataTable readCSVSIC(string filepath)

        {
            var SICList = new DataTable();
            foreach (var headerLine in File.ReadLines(filepath).Take(1))
            {
                foreach (var headerItem in headerLine.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    SICList.Columns.Add(headerItem.Trim().Replace("\"", "")
                        .Replace("Name", "Name")
                        .Replace("SIC", "SIC"));

                }
            }

            foreach (var line in File.ReadLines(filepath).Skip(1))
            {
                SICList.Rows.Add(line.Replace("\"", "").Split(','));
            }
            return SICList;
        }


        //public void LoadDaily()
        //{
        //    string CurrentUser = Globals.Username;
        //    try
        //    {
        //        using DataTable dt = readCSV("C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\612d239751dd5a85_-5362eb36_18b5c897a7f_10e5.csv");
        //        if (dt.Rows.Count > 0)
        //        {
        //            DataGrid1.ItemsSource = null;
        //            DataGrid1.ItemsSource = dt.DefaultView;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.MessageBox.Show(ex.Message, "Error");
        //    }
        //}


        void InfoGetAppData()
        {
            MainFormSub1Title.Text = "From 01/01/" + DateTime.Now.Year + "– 31/12/" + DateTime.Now.Year + ")";

            ContactNameTXT.Document.Blocks.Clear();
            ContactNameTXT.AppendText(AppDataWTN.ContactName);

            ProducerTXT.Document.Blocks.Clear();
            ProducerTXT.AppendText("Producer");

            EnvPermitYN.Document.Blocks.Clear();
            EnvPermitYN.AppendText("No");

            EnvNo.Document.Blocks.Clear();
            EnvNo.AppendText("N/A");

            EnvIssue.Document.Blocks.Clear();
            EnvIssue.AppendText("N/A");

            CompanyTXT.Document.Blocks.Clear();
            CompanyTXT.AppendText(AppDataWTN.CompanyName + "\r");
            CompanyTXT.AppendText(AppDataWTN.Address1 + "\r");
            CompanyTXT.AppendText(AppDataWTN.Town + "\r");

            PCTXT.Document.Blocks.Clear();
            PCTXT.AppendText(AppDataWTN.Postcode);

            PUKAddressTXT.Document.Blocks.Clear();
            PUKAddressTXT.AppendText("Polythene UK Ltd \r");
            PUKAddressTXT.AppendText("4 Witan Park, Avenue Two \r");
            PUKAddressTXT.AppendText("Witney \r");

            CollectAddTXT.Document.Blocks.Clear();
            CollectAddTXT.AppendText("N/A \r");
            CollectAddTXT.AppendText(" \r");
            CollectAddTXT.AppendText(" \r");

            BrokerTXT.Document.Blocks.Clear();
            BrokerTXT.AppendText("N/A \r");
            BrokerTXT.AppendText(" \r");
            BrokerTXT.AppendText(" \r");

            CusSigTXT.Document.Blocks.Clear();
            CusSigTXT.AppendText(AppDataWTN.ContactName);
            var rng = new TextRange(CusSigTXT.Document.ContentStart, CusSigTXT.Document.ContentEnd);
            rng.ApplyPropertyValue(Inline.FontStyleProperty, "Italic");
            rng.ApplyPropertyValue(Inline.FontFamilyProperty, "Lucida Handwriting");

            CusNameTXT.Document.Blocks.Clear();
            CusNameTXT.AppendText(AppDataWTN.ContactName);

            CusCompNameTXT.Document.Blocks.Clear();
            CusCompNameTXT.AppendText(AppDataWTN.CompanyName);

            DateTXT.Document.Blocks.Clear();
            DateTXT.AppendText(AppDataWTN.JobDate);

            WeightTXT.Document.Blocks.Clear();
            WeightTXT.AppendText(AppDataWTN.JobWeight);

            VisitIDTXT.Document.Blocks.Clear();
            VisitIDTXT.AppendText(AppDataWTN.JobID);

            String filepathSIC = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\data\\SIC\\SIC.csv";
            DataTable SICList = readCSVSIC(filepathSIC);
            SICTXT.Document.Blocks.Clear();

            foreach (DataRow row in SICList.Rows)
            {
                if (row[0].ToString() == AppDataWTN.CompanyName)
                {
                    SICTXT.Document.Blocks.Clear();
                    SICTXT.AppendText(row[1].ToString());
                }
            }

            String PostCode = AppDataWTN.Postcode.Trim();
            var PostCodeIndex = PostCode.Split(' ');
            String PostCodePrefix = PostCodeIndex[0];
            int PostCodeChar = PostCodePrefix.Count();

            Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
            Match result = re.Match(PostCodePrefix);

            String AlphaPostCode = result.Groups[1].Value;

            String filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\data\\Councils\\" + AlphaPostCode + ".csv";

            DataTable CouncilListCode = readCSV(filepath);

            foreach (DataRow row in CouncilListCode.Rows)
            {
                if (row[0].ToString().Substring(0, PostCodeChar) == PostCodePrefix)
                {
                    CouncilTXT.Document.Blocks.Clear();
                    CouncilTXT.AppendText(row[2].ToString());
                    return;
                }
            }


            //ContactNumberTXT.Text = AppDataWTN.ContactNumber;
            //ContactEmailTXT.Text = AppDataWTN.ContactEmail;
        }

        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            //hide some stuff

            //set light theme
            AppTheme.ChangeTheme(new Uri("Theme/AppLight.xaml", UriKind.Relative));



            System.Windows.Controls.PrintDialog dialog = new System.Windows.Controls.PrintDialog();
            if (dialog.ShowDialog() == true)
            {

                //get printer capabilities
                System.Printing.PrintCapabilities capabilities = dialog.PrintQueue.GetPrintCapabilities(dialog.PrintTicket);

                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / this.ActualWidth, capabilities.PageImageableArea.ExtentHeight / this.ActualHeight);
                this.LayoutTransform = new ScaleTransform(scale, scale);
                System.Windows.Size sz = new System.Windows.Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
                this.Measure(sz);
                this.Arrange(new Rect(new System.Windows.Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));
                dialog.PrintVisual(this, "Info Grid");
            }

            //show some stuff
            //dialog.ShowDialog();

            //set original theme
            //LoadTheme();

            //Update CI number on Print Press
            //string CurrentUser = Globals.Username;
            //String filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\data\\CommInvNumber.txt";
            //var ComInvNum = Convert.ToDouble(File.ReadAllText(filepath)) + 1;
            //File.WriteAllText(filepath, ComInvNum.ToString());



        }

    }
}
