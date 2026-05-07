using MySql.Data.MySqlClient;
using PolyUKApp.SQL;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for VanVisitListWindow.xaml
    /// </summary>
    public partial class VanVisitListWindow : Window
    {
        private readonly List<Window> _childWindows = new List<Window>();

        public VanVisitListWindow()
        {
            InitializeComponent();
            VanOldList();
        }

        private void VanVisitListWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Normal || this.WindowState == WindowState.Maximized)
            {
                this.Activate();

                // Restore in open-order; last one ends up on top
                _childWindows.RemoveAll(w => !w.IsLoaded);
                foreach (var win in _childWindows)
                {
                    win.WindowState = this.WindowState;
                    win.Activate();
                }
            }
        }

        private void OpenChildWindow(Window newWindow)
        {
            newWindow.Owner = this;

            this.IsHitTestVisible = false;

            // When child closes, return focus to main window
            newWindow.Closed += (s, e) =>
            {
                _childWindows.Remove(newWindow);
                _childWindows.RemoveAll(w => !w.IsLoaded);

                // Only re-enable if all child windows are closed
                if (!_childWindows.Any())
                    this.IsHitTestVisible = true;

                this.Activate();
            };

            _childWindows.RemoveAll(w => !w.IsLoaded);
            _childWindows.Add(newWindow);
            newWindow.Show();
        }

        private void BtnValidate_Click(object sender, RoutedEventArgs e)
        {
            var VisitCellInfo = VanDataGrid.SelectedCells[12];
            var IDName = Convert.ToInt32((VisitCellInfo.Column.GetCellContent(VisitCellInfo.Item) as TextBlock).Text);
            var connectionString = DataAccess.GlobalSQL.ConnectionMySQLVan;

            using (MySqlConnection _con = new MySqlConnection(connectionString))
            {
                var CommandStatement = DataAccess.GlobalSQLNonQueries.UNCompleteFromVanList;
                using (MySqlCommand _cmd = new MySqlCommand(CommandStatement, _con))
                {
                    _cmd.Parameters.AddWithValue("IDTEXT", IDName);

                    _con.Open();
                    _cmd.ExecuteNonQuery();
                    _con.Close();
                }
                VanOldList();
            }

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void VanOldList()
        {
            var connectionString = DataAccess.GlobalSQL.ConnectionMySQLVan;
            DataTable OldTable = new DataTable();

            using (MySqlConnection _con = new MySqlConnection(connectionString))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.VanListOLD;
                using (MySqlCommand _cmd = new MySqlCommand(queryStatement, _con))
                {
                    MySqlDataAdapter _dap = new MySqlDataAdapter(_cmd);
                    _con.Open();
                    _dap.Fill(OldTable);
                    _con.Close();
                }
                ComboBoxSearch.ItemsSource = OldTable.Columns;
                VanDataGrid.ItemsSource = null;
                VanDataGrid.ItemsSource = OldTable.DefaultView;
            }
        }

        private void VanDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var VisitCellInfo = VanDataGrid.SelectedCells[12];
            var IDName = (VisitCellInfo.Column.GetCellContent(VisitCellInfo.Item) as TextBlock).Text;
            System.Windows.Clipboard.Clear();
            System.Windows.Clipboard.SetDataObject(IDName.ToString());

            var thisWindow = Window.GetWindow(this);

            // Prevent duplicates - bring to focus if already open
            var existing = _childWindows.OfType<CompanyInfoWindow>().FirstOrDefault(w => w.IsLoaded);
            if (existing != null) { existing.Activate(); return; }
            var VisitInfoBox = new VanVisitInfoWindow
            {
                Left = thisWindow.Left,
                Top = thisWindow.Top,
                Height = thisWindow.Height,
                Width = thisWindow.Width
            };

            if (thisWindow.WindowState == WindowState.Maximized)
                VisitInfoBox.WindowState = WindowState.Maximized;

            OpenChildWindow(VisitInfoBox);



            //double WindowLeft = ThisWindow.Left;
            //double WindowTop = ThisWindow.Top;
            //double WindowHeight = ThisWindow.Height;
            //double WindowWidth = ThisWindow.Width;

            //if (ThisWindow.WindowState == WindowState.Maximized)
            //{
            //    var VisitInfoBox = new VanVisitInfoWindow();
            //    VisitInfoBox.WindowState = WindowState.Maximized;
            //    VisitInfoBox.Show();
            //}
            //else
            //{
            //    var VisitInfoBox = new VanVisitInfoWindow { Left = WindowLeft, Top = WindowTop, Width = WindowWidth, Height = WindowHeight };
            //    VisitInfoBox.Show();
            //}
        }

        private void BtnResetJobs_Click(object sender, RoutedEventArgs e)
        {
            BindingSource bsR = new BindingSource
            {
                DataSource = VanDataGrid.ItemsSource,
                Filter = ""
            };
            VanDataGrid.ItemsSource = bsR;
            ComboBoxSearch.Text = "";
            TxtBxSearch.Text = "";
            TextBlockComboError.Visibility = Visibility.Hidden;
        }

        private void BtnSearchJobs_Click(object sender, RoutedEventArgs e)
        {
            string SearchColumn = ComboBoxSearch.Text;
            if (SearchColumn != "")
            {
                TextBlockComboError.Visibility = Visibility.Hidden;
                BindingSource bs = new BindingSource
                {
                    DataSource = VanDataGrid.ItemsSource,
                    Filter = "[" + SearchColumn + "]" + " like '%" + TxtBxSearch.Text + "%'"
                };
                VanDataGrid.ItemsSource = bs;
            }
            else
            {
                TextBlockComboError.Visibility = Visibility.Visible;
            }
        }
    }
}
