using Microsoft.VisualBasic.ApplicationServices;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using PolyUKApp.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PolyUKApp.Windows
{
    /// <summary>
    /// Interaction logic for VanMapWindow.xaml
    /// </summary>
    public partial class VanMapWindow : Window
    {
        public VanMapWindow()
        {
            InitializeComponent();
            MySQLGetVan();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public void LoadMap()
        {
            
            WebView.Source = new Uri(@"");

        }

        public void MySQLGetVan()
        {
            var ConnectionString = DataAccess.GlobalSQL.ConnectionMySQLVan;
            DataTable VanList = new DataTable();

            using (MySqlConnection _con = new MySqlConnection(ConnectionString))
            {
                var QueryStatement = DataAccess.GlabalSQLQueries.VanList;
                using (MySqlCommand _cmd = new MySqlCommand(QueryStatement, _con))
                {
                    MySqlDataAdapter _dap = new MySqlDataAdapter(_cmd);
                    _con.Open();
                    _dap.Fill(VanList);
                    _con.Close();

                    GeoGrid.ItemsSource = VanList.DefaultView;


                }
            }
        }

        public void GeocodeStart()
        {

        }

    }
}
