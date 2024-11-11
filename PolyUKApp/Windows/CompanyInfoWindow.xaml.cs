using System;
using System.Collections.Generic;
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
    /// Interaction logic for CompanyInfoWindow.xaml
    /// </summary>
    public partial class CompanyInfoWindow : Window
    {
        public CompanyInfoWindow()
        {
            InitializeComponent();
            FillCompanyInfo();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void FillCompanyInfo()
        {
            var CompanyInfoText = "Company Details \n" +
            "Polythene Uk Ltd\r" +
            "31c Avenue One, Station Lane, Witney, OX28 4XZ\n" +
            "Reg: 06039291\r" +
            "VAT: GB903824828\r" +
            "GB EORI: GB903824828000\r" +
            "EU EORI: XI903824828000\n" +
            "T: 01993 777950\r" +
            "E: info@polytheneuk.co.uk\n" +
            "RBS Bank Details:\r" +
            "Sort: 60-08-46\r" +
            "Acc: 71691359\r" +
            "IBAN: GB17NWBK60084671691359\r" +
            "BIC: NWBKGB2L\n" +
            "Euro Bank Details:\r" +
            "Sort: 60-24-60\r" +
            "Acc: 550/00/18511684\r" +
            "IBAN: GB68NWBK60720118511864\r" +
            "BIC: NWBKGB2L\n" +
            "Proforma Bank Details:\r" +
            "Sort: 60-24-60\r" +
            "Acc: 15614565\r" +
            "IBAN: GB79NWBK60246015614565\r" +
            "SWIFT: NWBKGB2L";
            RichTextCompany.AppendText(CompanyInfoText);
            
        }
    }
}
