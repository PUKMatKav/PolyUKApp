using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PolyUKApp.Windows
{
    /// <summary>
    /// Interaction logic for PicWindow.xaml
    /// </summary>
    public partial class PicWindow : Window
    {
        protected bool validData;
        string path;
        protected System.Drawing.Image image;
        protected Thread getImageThread;

        public PicWindow()
        {
            InitializeComponent();
            LoadVisit();
        }

        public void LoadVisit()
        {
            System.Windows.IDataObject DataID = System.Windows.Clipboard.GetDataObject();
            string VisitID = (String)DataID.GetData(typeof(String));
            JobTitleIDText.Text = VisitID;

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MainImage_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.

                BitmapImage img = new BitmapImage(new System.Uri(files[0]));
                MainImage.Source = img;
            }
        }

        private void HandleFileOpen()
        {
            Uri src = new Uri(@"/ComponentName;component/Images/logo.png", UriKind.Relative);
            BitmapImage img = new BitmapImage(src);
            MainImage.Source = img;
        }
    }
}

