using Microsoft.Data.SqlClient;
using PolyUKApp.SQL;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Forms;


namespace PolyUKApp.Windows
{
    /// <summary>
    /// Interaction logic for StockItemWindow.xaml
    /// </summary>
    public partial class StockItemWindow : Window
    {
        public StockItemWindow()
        {
            InitializeComponent();
            SqlConnectStock();
        }
        public void SqlConnectStock()
        {
            string connectionString = DataAccess.GlobalSQL.Connection;
            DataTable stockItemTable = new DataTable("StockItemList");

            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                string queryStatement = DataAccess.GlabalSQLQueries.ItemListQuery;

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);

                    _con.Open();
                    _dap.Fill(stockItemTable);
                    _con.Close();
                    ComboBoxSearch.ItemsSource = stockItemTable.Columns;
                    DataGridStock.ItemsSource = null;
                    DataGridStock.ItemsSource = stockItemTable.DefaultView;
                }
            }
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
        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var displayName = GetPropertyDisplayName(e.PropertyDescriptor);

            if (!string.IsNullOrEmpty(displayName))
            {
                e.Column.Header = displayName;
            }

        }
        public static string GetPropertyDisplayName(object descriptor)
        {
            var pd = descriptor as PropertyDescriptor;

            if (pd != null)
            {
                // Check for DisplayName attribute and set the column header accordingly
                var displayName = pd.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;

                if (displayName != null && displayName != DisplayNameAttribute.Default)
                {
                    return displayName.DisplayName;
                }

            }
            else
            {
                var pi = descriptor as PropertyInfo;

                if (pi != null)
                {
                    // Check for DisplayName attribute and set the column header accordingly
                    Object[] attributes = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    for (int i = 0; i < attributes.Length; ++i)
                    {
                        var displayName = attributes[i] as DisplayNameAttribute;
                        if (displayName != null && displayName != DisplayNameAttribute.Default)
                        {
                            return displayName.DisplayName;
                        }
                    }
                }
            }

            return null;
        }

        private async void DataGridStock_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var cellInfo = DataGridStock.CurrentCell;
            {
                var column = cellInfo.Column as DataGridBoundColumn;
                if (column != null)
                {
                    var element = new FrameworkElement() { DataContext = cellInfo.Item };
                    BindingOperations.SetBinding(element, TagProperty, column.Binding);
                    var cellValue = element.Tag;
                    System.Windows.Clipboard.Clear();
                    System.Windows.Clipboard.SetText(cellValue.ToString());
                    TextBlockCopied.Visibility = Visibility.Visible;
                    await Task.Delay(2000);
                    TextBlockCopied.Visibility = Visibility.Hidden;
                }
            }
        }

        private void DataGridStock_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnItemView.Visibility = Visibility.Visible;
        }

        private void BtnSearchStock_Click(object sender, RoutedEventArgs e)
        {
            string SearchColumn = ComboBoxSearch.Text;

            if (SearchColumn != "")
            {
                TextBlockComboError.Visibility = Visibility.Hidden;
                BindingSource bs = new BindingSource
                {
                    DataSource = DataGridStock.ItemsSource,
                    Filter = "[" + SearchColumn + "]" + " like '%" + TxtBxSearch.Text + "%'"
                };
                DataGridStock.ItemsSource = bs;
            }
            else
            {
                TextBlockComboError.Visibility = Visibility.Visible;
            }
        }

        private void BtnResetStock_Click(object sender, RoutedEventArgs e)
        {
            BindingSource bsR = new BindingSource
            {
                DataSource = DataGridStock.ItemsSource,
                Filter = ""
            };
            DataGridStock.ItemsSource = bsR;
            ComboBoxSearch.Text = "";
            TxtBxSearch.Text = "";
            BtnItemView.Visibility = Visibility.Hidden;
        }

        private void BtnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnItemView_Click(object sender, RoutedEventArgs e)
        {
            var CellInfo = DataGridStock.SelectedCells[0];
            var ItemCode = (CellInfo.Column.GetCellContent(CellInfo.Item) as TextBlock).Text;
            System.Windows.Clipboard.Clear();
            System.Windows.Clipboard.SetText(ItemCode.ToString());

            var ThisWindow = Window.GetWindow(this);

            double WindowLeft = ThisWindow.Left;
            double WindowTop = ThisWindow.Top;
            double WindowHeight = ThisWindow.Height;
            double WindowWidth = ThisWindow.Width;

            if (ThisWindow.WindowState == WindowState.Maximized)
            {
                var StockCodeBox = new StockCodeViewWindow();
                StockCodeBox.WindowState = WindowState.Maximized;
                StockCodeBox.Show();
            }
            else
            {
                var StockItemBox = new StockCodeViewWindow { Left = WindowLeft, Top = WindowTop, Height = WindowHeight, Width = WindowWidth };
                StockItemBox.Show();
            }
        }
    }
}
