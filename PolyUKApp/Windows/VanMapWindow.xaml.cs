using Microsoft.VisualBasic.ApplicationServices;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
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
            LoadMap();


        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public void LoadMap()
        {
            
            WebView.Source = new Uri(@"http://stackoverflow.com");

        }

    }
}
