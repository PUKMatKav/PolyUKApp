﻿using PolyUKApp.Windows;
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
            else TextBlockWelcome.AppendText("Hello unkown user!");
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
                "This may not fit on smaller screens currently!" );
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
    }
}