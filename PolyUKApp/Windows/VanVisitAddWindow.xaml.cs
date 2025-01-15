using MySql.Data.MySqlClient;
using PolyUKApp.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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
using static PolyUKApp.Windows.CallTimeWindow;

namespace PolyUKApp.Windows
{
    /// <summary>
    /// Interaction logic for VanVisitAddWindow.xaml
    /// </summary>
    public partial class VanVisitAddWindow : Window
    {
        public VanVisitAddWindow()
        {
            InitializeComponent();
            ComboFiller();
            RandomGen();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RandomGen()
        {
            Random rnd = new Random();
            var IDNumber = rnd.Next(1,999999);
            TextVisitID.AppendText(IDNumber.ToString());
        }

        private void ComboFiller()
        {
            ComboSalesStaff.Items.Add("Donna Rivera");
            ComboSalesStaff.Items.Add("Jack Mungall");
            ComboSalesStaff.Items.Add("James Woollard");
            ComboSalesStaff.Items.Add("Jason Mayhew");
            ComboSalesStaff.Items.Add("Natalie Horler");
            ComboSalesStaff.Items.Add("Neerisha Singh");
            ComboSalesStaff.Items.Add("Ryan King");
            ComboSalesStaff.Items.Add("Tom Matthews");

            ComboAdminStaff.Items.Add("Ant");
            ComboAdminStaff.Items.Add("Jake");
            ComboAdminStaff.Items.Add("Both");
            ComboAdminStaff.Items.Add("James");

            ComboType.Items.Add("Technical");
            ComboType.Items.Add("Collection");
            ComboType.Items.Add("Delivery");
            ComboType.Items.Add("Part Collected");
            ComboType.Items.Add("Sales");
            ComboType.Items.Add("Other");

            ComboPromTime.Items.Add("08:00");
            ComboPromTime.Items.Add("08:30");
            ComboPromTime.Items.Add("09:00");
            ComboPromTime.Items.Add("09:30");
            ComboPromTime.Items.Add("10:00");
            ComboPromTime.Items.Add("10:30");
            ComboPromTime.Items.Add("11:00");
            ComboPromTime.Items.Add("11:30");
            ComboPromTime.Items.Add("12:00");
            ComboPromTime.Items.Add("12:30");
            ComboPromTime.Items.Add("13:00");
            ComboPromTime.Items.Add("13:30");
            ComboPromTime.Items.Add("14:00");
            ComboPromTime.Items.Add("14:30");
            ComboPromTime.Items.Add("15:00");
            ComboPromTime.Items.Add("15:30");
            ComboPromTime.Items.Add("16:00");
            ComboPromTime.Items.Add("16:30");

            ComboJobTime.Items.Add("0.5");
            ComboJobTime.Items.Add("1.0");
            ComboJobTime.Items.Add("1.5");
            ComboJobTime.Items.Add("2.0");
            ComboJobTime.Items.Add("2.5");
            ComboJobTime.Items.Add("3.0");
            ComboJobTime.Items.Add("3.5");
            ComboJobTime.Items.Add("4.0");
            ComboJobTime.Items.Add("4.5");
            ComboJobTime.Items.Add("5.0");
            ComboJobTime.Items.Add("5.5");
            ComboJobTime.Items.Add("6.0");
            ComboJobTime.Items.Add("6.5");
            ComboJobTime.Items.Add("7.0");
            ComboJobTime.Items.Add("7.5");
            ComboJobTime.Items.Add("8.0");
            ComboJobTime.Items.Add("8.5");

            ComboCreditChecked.Items.Add("No");
            ComboCreditChecked.Items.Add("Yes");
        }

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDate = StartDatePicker.SelectedDate;
            if (selectedDate != null)
            {
                RichTextPromDate.Document.Blocks.Clear();
                RichTextPromDate.AppendText(selectedDate.ToString().Substring(0, 10));
                RichTextDatePotential.Document.Blocks.Clear();
                RichTextDatePotential.AppendText("Not Saved");
            }
            else
            {
                RichTextDatePotential.Document.Blocks.Clear();
            }
        }

        private void AddVisit()
        {
            //big boy button, must control user usage!!

            String COText = VisitTextBox.Text;
            String AddressText = TextBoxCusAddInfo.Text;
            String PostcodeText = TextBoxPostcode.Text;
            var DescRange = new TextRange(RichTextVisitDesc.Document.ContentStart, RichTextVisitDesc.Document.ContentEnd);
            String DescText = DescRange.Text.Replace("\r", "").Replace("\n", "");
            String NameText = TextContactName.Text;
            String NumberText = TextContactNum.Text;
            String EmailText = TextContactEmail.Text;
            String SalesText = ComboSalesStaff.Text.ToString();
            String StaffText = ComboAdminStaff.Text.ToString();
            String VisitText = ComboType.Text.ToString();
            String TownText = TextBoxTown.Text;
            int IDText = Convert.ToInt32(TextVisitID.Text.ToString().Replace("\r", "").Replace("\n", ""));
            var CreditCheckedText = ComboCreditChecked.Text.ToString();
            var PlannedStartText = ComboPromTime.Text.ToString();
            var JobTimeText = ComboJobTime.Text.ToString();
            var CompanyRegRange = new TextRange(RichTextRegNo.Document.ContentStart, RichTextRegNo.Document.ContentEnd);
            String CompanyReg = CompanyRegRange.Text.Replace("\r", "").Replace("\n", "");
            var AnnualTurnoverRange = new TextRange(RichTextTurnover.Document.ContentStart, RichTextTurnover.Document.ContentEnd);
            String AnnualTurnover = AnnualTurnoverRange.Text.Replace("\r", "").Replace("\n", "");

            string connectionString = DataAccess.GlobalSQL.ConnectionMySQLVan;

            using (MySqlConnection _con = new MySqlConnection(connectionString))
            {

                var CommandStatement = DataAccess.GlobalSQLNonQueries.AddVanList;
                using (MySqlCommand _cmd = new MySqlCommand(CommandStatement, _con))
                {
                    var DateRange = new TextRange(RichTextPromDate.Document.ContentStart, RichTextPromDate.Document.ContentEnd);
                    String DateText = DateRange.Text.Replace("\r", "").Replace("\n", "");

                    _cmd.Parameters.AddWithValue("@AddressText", AddressText);
                    _cmd.Parameters.AddWithValue("@TownText", TownText);
                    _cmd.Parameters.AddWithValue("@PostcodeText", PostcodeText);
                    _cmd.Parameters.AddWithValue("@DescText", DescText);
                    _cmd.Parameters.AddWithValue("@NameText", NameText);
                    _cmd.Parameters.AddWithValue("@EmailText", EmailText);
                    _cmd.Parameters.AddWithValue("@NumberText", NumberText);
                    _cmd.Parameters.AddWithValue("@SalesText", SalesText);
                    _cmd.Parameters.AddWithValue("@StaffText", StaffText);
                    _cmd.Parameters.AddWithValue("@VisitText", VisitText);
                    _cmd.Parameters.AddWithValue("@PlannedDate", DateText);
                    _cmd.Parameters.AddWithValue("@COText", COText);
                    _cmd.Parameters.AddWithValue("@CreditCheckedText", CreditCheckedText);
                    _cmd.Parameters.AddWithValue("@PlannedStartText", PlannedStartText);
                    _cmd.Parameters.AddWithValue("@JobTimeText", JobTimeText);
                    _cmd.Parameters.AddWithValue("@Turnover", AnnualTurnover);
                    _cmd.Parameters.AddWithValue("@CompanyReg", CompanyReg);

                    _cmd.Parameters.AddWithValue("@IDTEXT", IDText);

                    _con.Open();
                    _cmd.ExecuteNonQuery();
                    _con.Close();

                }
                RichTextDatePotential.Document.Blocks.Clear();
                System.Windows.MessageBox.Show("Visit Added!");
            }
        }

        private void BtnValidate_Click(object sender, RoutedEventArgs e)
        {
            AddVisit();
            Close();
        }

    }
}
