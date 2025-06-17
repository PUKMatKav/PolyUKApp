using Microsoft.Exchange.WebServices.Data;
using PolyUKApp.Windows;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Application = System.Windows.Application;

namespace PolyUKApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            LoadTheme();
            CurrentDateDisplay();

        }
        private void MainWindow_SizeChanged()
        {
            double WindowHeight = System.Windows.Application.Current.MainWindow.Height;
            if (WindowHeight > 800)
            {
                ExtraBorder.Visibility = Visibility.Visible;
            }
            else { ExtraBorder.Visibility = Visibility.Collapsed; }
        }

        private void BtnCallTime_Click(object sender, RoutedEventArgs e)
        {
            var CallTimerWindow = new CallTimeWindow();
            CallTimerWindow.Closed += childFormClosed;
            CallTimerWindow.Show();
            this.Hide();
        }
        void childFormClosed(object sender, EventArgs e)
        {
            ((CallTimeWindow)sender).Closed -= childFormClosed;
            this.Show();
        }

        private void BtnWorksOrders_Click(object sender, RoutedEventArgs e)
        {
            var WOCalendarWindow = new WOCalendarWindow();
            WOCalendarWindow.Closed += childFormWOCalendarClosed;
            WOCalendarWindow.Show();
            this.Hide();
        }
        void childFormWOCalendarClosed(object sender, EventArgs e)
        {
            ((WOCalendarWindow)sender).Closed -= childFormWOCalendarClosed;
            this.Show();
        }

        private void BtnVanCalendar_Click(object sender, RoutedEventArgs e)
        {
            var VanCalWindow = new VanCalendarWindow();
            VanCalWindow.Closed += childFormVanCalClosed;
            VanCalWindow.Show();
            this.Hide();
        }
        void childFormVanCalClosed(object sender, EventArgs e)
        {
            ((VanCalendarWindow)sender).Closed -= childFormVanCalClosed;
            this.Show();
        }

        private void BtnDatabaseViewer_Click(object sender, RoutedEventArgs e)
        {
            var DatabaseWindow = new DatabaseWindow();
            DatabaseWindow.Closed += childFormDatabaseClosed;
            DatabaseWindow.Show();
            this.Hide();
        }
        void childFormDatabaseClosed(object sender, EventArgs e)
        {
            ((DatabaseWindow)sender).Closed -= childFormDatabaseClosed;
            this.Show();
        }

        private void BtnStockViewer_Click(object sender, RoutedEventArgs e)
        {
            var StockItemWindow = new StockItemWindow();
            StockItemWindow.Closed += childFormStockViewerClosed;
            StockItemWindow.Show();
            this.Hide();
        }
        void childFormStockViewerClosed(object sender, EventArgs e)
        {
            ((StockItemWindow)sender).Closed -= childFormStockViewerClosed;
            this.Show();
        }

        private void BtnCompanyInfo_Click(object sender, RoutedEventArgs e)
        {
            double WindowLeft = System.Windows.Application.Current.MainWindow.Left;
            double WindowTop = System.Windows.Application.Current.MainWindow.Top;
            double WindowHeight = System.Windows.Application.Current.MainWindow.Height;
            double WindowWidth = System.Windows.Application.Current.MainWindow.Width;
            var thisWindow = Application.Current.MainWindow;
            if (thisWindow.WindowState == WindowState.Maximized)
            {
                var CompanyInfoBox = new CompanyInfoWindow();
                CompanyInfoBox.WindowState = WindowState.Maximized;
                CompanyInfoBox.Show();
            }
            else
            {
                var CompanyInfoBox = new CompanyInfoWindow { Left = WindowLeft, Top = WindowTop, Height = WindowHeight, Width = WindowWidth };
                CompanyInfoBox.Show();
            }


        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
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

        private void CurrentDateDisplay()
        {
            var DayName = DateTime.Now.DayOfWeek.ToString();
            var DayNumber = DateTime.Now.Day.ToString();
            var MonthName = DateTime.Now.ToString("MMMM");
            var YearNumber = DateTime.Now.ToString("yyyy");
            TextBlockDate.Text = DayName + " " + DayNumber + " " + MonthName + " " + YearNumber;
        }

        private void TextBlockWelcome_Loaded(object sender, RoutedEventArgs e)
        {
            string loginname = Environment.UserName;
            if (loginname == "MatthewKavanagh")
            {
                TextBlockWelcome.AppendText("Hello Matt K");
            }
            else if (loginname == "M.McSherry")
            {
                TextBlockWelcome.AppendText("Hello Meg...");
            }
            else if (loginname == "SadieAndrews")
            {
                TextBlockWelcome.AppendText("Hello Sadie!");
            }
            else if (loginname == "MatthewDewe")
            {
                TextBlockWelcome.AppendText("Hello Matt D!");
            }
            else if (loginname == "SophieGroth")
            {
                TextBlockWelcome.AppendText("Hello Sophie!");
            }
            else if (loginname == "KylieWoollard")
            {
                TextBlockWelcome.AppendText("Hello Kylie!");
            }
            else if (loginname == "JamesWoollard")
            {
                TextBlockWelcome.AppendText("Hello James!");
            }
            else if (loginname == "TomMatthews")
            {
                TextBlockWelcome.AppendText("Hello Tom!");
            }
            else TextBlockWelcome.AppendText("Hello new user!");
        }

        private void BtnCallTime_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TextBlockInfo.Document.Blocks.Clear();
            TextBlockInfo.AppendText("Display daily and weekly call time, now with the ability to download directly from Akixi's server (depending on the time!)");
        }

        private void BtnWorksOrders_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TextBlockInfo.Document.Blocks.Clear();
            TextBlockInfo.AppendText("Show works order calendar and list of currently live works orders being completed downstairs.\n" +
                "This may not fit on smaller screens currently!");
        }

        private void BtnDatabaseViewer_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TextBlockInfo.Document.Blocks.Clear();
            TextBlockInfo.AppendText("Show various SAGE databases with the ability to filter by various catagories.");
        }

        private void BtnStockViewer_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TextBlockInfo.Document.Blocks.Clear();
            TextBlockInfo.AppendText("Display and view stock codes from SAGE as well as filter the information based on certain keywords.");
        }

        private void BtnCompanyInfo_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TextBlockInfo.Document.Blocks.Clear();
            TextBlockInfo.AppendText("Display company information");
        }

        private void BtnExit_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TextBlockInfo.Document.Blocks.Clear();
            TextBlockInfo.AppendText("Exit Program");
        }

        private void TextBlockFact_Loaded(object sender, RoutedEventArgs e)
        {
            var CurrentUser = Environment.UserName;
            var filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\data\\trivia.txt";
            var filepathMCD = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Matt K Stuff\\data\\beemovie.txt";

            if (CurrentUser == "M.McSherry")
            {
                List<String> TriviaListMCD = System.IO.File.ReadAllLines(filepathMCD).ToList();
                int linecountMCD = TriviaListMCD.Count;

                Random rndMCD = new Random();
                int numberrollMCD = rndMCD.Next(0, linecountMCD);
                int lineMCD = numberrollMCD;
                TextBlockFact.AppendText("Bee Movie Line " + lineMCD + " - " + TriviaListMCD[lineMCD]);
            }
            else
            {
                List<String> TriviaList = System.IO.File.ReadAllLines(filepath).ToList();
                int linecount = TriviaList.Count;

                Random rnd = new Random();
                int numberroll = rnd.Next(0, linecount);
                int line = numberroll;
                TextBlockFact.AppendText("Fun Fact: " + TriviaList[line]);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MainWindow_SizeChanged();
        }

        private void BtnVanCalendar_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TextBlockInfo.Document.Blocks.Clear();
            TextBlockInfo.AppendText("Allows you to view a calendar with the current planned van jobs, as well as the list of all outstanding jobs (that don't have a planned date yet). Also lets certain people edit the jobs, add new jobs or delete exisiting jobs.");
        }

        private void BtnStockOrdering_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TextBlockInfo.Document.Blocks.Clear();
            TextBlockInfo.AppendText("Table of general sale stock and amounts on order for seeing what is in need of topping up.");
        }

        private void BtnStockOrdering_Click(object sender, RoutedEventArgs e)
        {
            var StockOrderWindow = new StockOrderingWindow();
            StockOrderWindow.Closed += childFormStockOrderClosed;
            StockOrderWindow.Show();
            this.Hide();
        }
        void childFormStockOrderClosed(object sender, EventArgs e)
        {
            ((StockOrderingWindow)sender).Closed -= childFormStockOrderClosed;
            this.Show();
        }

        private void versionbox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //v1.0.0.1 - Initial Release
            //v1.0.0.2 - Updating permissions for Van Calendar
            //v1.0.0.3 - Added to Github Distro
            //v1.0.0.4 - Rebuilt OneClick Launcher for Automatic update
            /*System.Windows.MessageBox.Show("v1.0.0.5" +
                "\r" + "" + "\r" +
                "- Reworked controls on van visit edit\n" +
                "- Added duplicate button for copying completed visits\n" +
                "- Added filter for viewing old visits\n" +
                "- Added base stock ordering monitor (WIP)");*/
            /*System.Windows.MessageBox.Show("v1.0.1.6" +
                "\r" + "" + "\r" +
                "- Reworked controls on van visit edit\n" +
                "- Removed Stock Ordering Button\n" +
                "- Added Commercial Invoice Generator\n" +
                "   - Can create CI from just using an order number\n" +
                "   - Pulls weight and item info along with address and generate EORI\n" +
                "   - Can be print to PDF from page and will update CI number for next CI\n" +
                "- Removed staff member from van calendar overview to fit 3 jobs per day");*/
            /*System.Windows.MessageBox.Show("v1.0.1.7" +
                "\r" + "" + "\r" +
                "- Added Annual Turnover to van visit info\n" +
                "- Added Credit Registration to van visit info\n");*/
            /*System.Windows.MessageBox.Show("v1.0.1.8" +
                "\r" + "" + "\r" +
                "- Better wording on van visit info\n");*/
            /*System.Windows.MessageBox.Show("v1.1.0.0" +
                "\r" + "" + "\r" +
                "- There is a dark mode now\n" +
                "- Theme settings should save for individual users\n" +
                "- Fixed duplicate van collection job crash\n" +
                "- Added amend button to completed van jobs\n");*/
            /*System.Windows.MessageBox.Show("v1.1.2.0k" +
                "\r" + "" + "\r" +
                "- Updated window fluidity for smaller screens (beta - van calendar)\n" +
                "- Updated permissions for van calendar buttons\n" +
                "- Included customer type drop down\n" +
                "- Updated database to include customer_type data field\n" +
                "- Small formatting changes to van calendar pop up screens\n" +
                "- (k) Adjusted user permissions again\n");*/

            /*System.Windows.MessageBox.Show("v1.2.0.0" +
                "\r" + "" + "\r" +
                "- Added Dark mode to Commercial Invoice page\n" +
                "- Rebuilt database for CI page\n" +
                "- Added draft function for CI generation\n" +
                "- Added better numbering system for new CI numbers\n" +
                "- Increased number of editable fields on CI page\n" +
                "- Added automatic recall of drafts for CIs already created\n" +
                "- Added extra info fields for CI creation" +
                "\r" + "" + "\r" +
                "v1.2.0.1 - removed call time download button for now\n" +
                "v1.2.0.2 - changed delimiter for CI draft saving to avoid comma confusion");*/

            System.Windows.MessageBox.Show("v1.2.1.0" +
                "\r" + "" + "\r" +
                "- Added more functionality to CI page\n" +
                "- CI Page now allow on the fly edits to qty\n" +
                "- Added file dialog for van calendar\n" +
                "- Allows for saving and viewing of images\n" +
                "- Auto creates folder for each job when images saved\n" +
                "- Some minor things I have done and long since forgotten\n" +
                "\r" + "" + "\r" +
                "v1.2.1.1 - Removed duplicate opening of save image dialog, also correctly checks if file saved on close\n");
        }

        private void BtnCommInvoice_Click(object sender, RoutedEventArgs e)
        {
            var CIWindow = new CommInvoiceWindow();
            CIWindow.Closed += childFormCommInvoiceClosed;
            CIWindow.Show();
            this.Hide();

        }

        void childFormCommInvoiceClosed(object sender, EventArgs e)
        {
            ((CommInvoiceWindow)sender).Closed -= childFormCommInvoiceClosed;
            this.Show();
        }



        private void BtnCommInvoice_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TextBlockInfo.Document.Blocks.Clear();
            TextBlockInfo.AppendText("Allows you to generate a commercial invoice just from the order number! Please double check the info fields are filled in though!");
        }

        private void BtnCallLink_Click(object sender, RoutedEventArgs e)
        {
            BtnCallTime_Click(sender, e);
        }

        private void BtnCallLink_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BtnCallTime_MouseEnter(sender, e);
        }

        private void BtnVanLink_Click(object sender, RoutedEventArgs e)
        {
            BtnVanCalendar_Click(sender, e);
        }

        private void BtnVanLink_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BtnVanCalendar_MouseEnter(sender, e);
        }

        private void BtnWOLink_Click(object sender, RoutedEventArgs e)
        {
            BtnWorksOrders_Click(sender, e);
        }

        private void BtnWOLink_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BtnWorksOrders_MouseEnter(sender, e);
        }

        private void BtnCILink_Click(object sender, RoutedEventArgs e)
        {
            BtnCommInvoice_Click(sender, e);
        }

        private void BtnCILink_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BtnCommInvoice_MouseEnter(sender, e);
        }

        private void BtnCompanyLink_Click(object sender, RoutedEventArgs e)
        {
            BtnCompanyInfo_Click(sender, e);
        }

        private void BtnCompanyLink_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BtnCompanyInfo_MouseEnter(sender, e);
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

        private void BtnPODs_Click(object sender, RoutedEventArgs e)
        {
            var PODWindow = new PODWindow();
            PODWindow.Closed += childFormPODsClosed;
            PODWindow.Show();
            this.Hide();
        }

        void childFormPODsClosed(object sender, EventArgs e)
        {
            ((PODWindow)sender).Closed -= childFormPODsClosed;
            this.Show();
        }

        private void BtnPODs_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TextBlockInfo.Document.Blocks.Clear();
            TextBlockInfo.AppendText("Can display outstanding PODs and draft emails to send to request them.");

        }


    }
}