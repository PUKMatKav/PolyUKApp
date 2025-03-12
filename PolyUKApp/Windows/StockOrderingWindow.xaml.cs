using Microsoft.Data.SqlClient;
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
    /// Interaction logic for StockOrderingWindow.xaml
    /// </summary>
    public partial class StockOrderingWindow : Window
    {
        public StockOrderingWindow()
        {
            InitializeComponent();
            SqlStockOrderConnection();
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

        private void BtnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SqlStockOrderConnection()
        {
            string connectionString = DataAccess.GlobalSQL.Connection;
            DataTable stockOrderTable = new DataTable("StockOrderList");

            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                string queryStatement = DataAccess.GlabalSQLQueries.StockOrderQuery;

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);

                    _con.Open();
                    _dap.Fill(stockOrderTable);
                    _con.Close();
                    DataGridStockOrderItems.ItemsSource = stockOrderTable.DefaultView;
                }
            }
        }

    }
}
