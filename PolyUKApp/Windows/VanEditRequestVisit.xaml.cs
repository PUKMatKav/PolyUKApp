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
using IDataObject = System.Windows.IDataObject;

namespace PolyUKApp.Windows
{
    /// <summary>
    /// Interaction logic for VanEditRequestVisit.xaml
    /// </summary>
    public partial class VanEditRequestVisit : Window
    {
        public VanEditRequestVisit()
        {
            InitializeComponent();
            buttonCheckerAccept();
            LoadVisit();
            ComboItems();
            VanInfoSQL();
        }

        public void ComboItems()
        {
            ComboCompanyType.Items.Add("Customer");
            ComboCompanyType.Items.Add("Prospect");

            
            ComboAdminStaff.Items.Add("Jake");
            ComboAdminStaff.Items.Add("Both");
            ComboAdminStaff.Items.Add("James");

            ComboType.Items.Add("Technical");
            ComboType.Items.Add("Collection");
            ComboType.Items.Add("Delivery");
            ComboType.Items.Add("Part Collected");
            ComboType.Items.Add("Sales");
            ComboType.Items.Add("Other");

            ComboSalesStaff.Items.Add("Donna Rivera");
            ComboSalesStaff.Items.Add("Jack Mungall");
            ComboSalesStaff.Items.Add("James Woollard");
            ComboSalesStaff.Items.Add("Jason Mayhew");
            ComboSalesStaff.Items.Add("Natalie Horler");
            ComboSalesStaff.Items.Add("Neerisha Singh");
            ComboSalesStaff.Items.Add("Ryan King");
            ComboSalesStaff.Items.Add("Tom Matthews");

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

        public void LoadVisit()
        {
            IDataObject DataID = System.Windows.Clipboard.GetDataObject();
            string VisitID = (String)DataID.GetData(typeof(String));
            RichTextVisitID.AppendText(VisitID);

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
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

        private void BtnValidate_Click(object sender, RoutedEventArgs e)
        {
            //big boy button!! Must Hide from most users

            String COText = VisitTextBox.Text;
            var AddressRange = new TextRange(RichTextCusAddInfo.Document.ContentStart, RichTextCusAddInfo.Document.ContentEnd);
            String AddressText = AddressRange.Text.Replace("\r", "").Replace("\n", "");
            var PostcodeRange = new TextRange(RichTextPostcode.Document.ContentStart, RichTextPostcode.Document.ContentEnd);
            String PostcodeText = PostcodeRange.Text.Replace("\r", "").Replace("\n", "");
            var DescRange = new TextRange(RichTextVisitDesc.Document.ContentStart, RichTextVisitDesc.Document.ContentEnd);
            String DescText = DescRange.Text.Replace("\r", "").Replace("\n", "");
            var NameRange = new TextRange(RichTextContactName.Document.ContentStart, RichTextContactName.Document.ContentEnd);
            String NameText = NameRange.Text.Replace("\r", "").Replace("\n", "");
            var NumberRange = new TextRange(RichTextContactNum.Document.ContentStart, RichTextContactNum.Document.ContentEnd);
            String NumberText = NumberRange.Text.Replace("\r", "").Replace("\n", "");
            var EmailRange = new TextRange(RichTextContactEmail.Document.ContentStart, RichTextContactEmail.Document.ContentEnd);
            String EmailText = EmailRange.Text.Replace("\r", "").Replace("\n", "");
            String SalesText = ComboSalesStaff.Text.ToString();
            String StaffText = ComboAdminStaff.Text.ToString();
            String VisitText = ComboType.Text.ToString();
            var TownRange = new TextRange(RichTextTown.Document.ContentStart, RichTextTown.Document.ContentEnd);
            String TownText = TownRange.Text.Replace("\r", "").Replace("\n", "");
            var IDRANGE = new TextRange(RichTextVisitID.Document.ContentStart, RichTextVisitID.Document.ContentEnd);
            int IDText = Convert.ToInt32(IDRANGE.Text.ToString().Replace("\r", "").Replace("\n", ""));
            var CreditCheckedText = ComboCreditChecked.Text.ToString();
            var PlannedStartText = ComboPromTime.Text.ToString();
            var JobTimeText = ComboJobTime.Text.ToString();
            var TurnoverRange = new TextRange(RichTextTurnover.Document.ContentStart, RichTextTurnover.Document.ContentEnd);
            String TurnoverText = TurnoverRange.Text.Replace("\r", "").Replace("\n", "");
            var CompanyRegRange = new TextRange(RichTextRegNo.Document.ContentStart, RichTextRegNo.Document.ContentEnd);
            String CompanyRegText = CompanyRegRange.Text.Replace("\r", "").Replace("\n", "");
            var CompanyType = ComboCompanyType.Text.ToString();

            string connectionString = DataAccess.GlobalSQL.ConnectionMySQLVan;

            using (MySqlConnection _con = new MySqlConnection(connectionString))
            {

                var CommandStatement = DataAccess.GlobalSQLNonQueries.UpdateVanPendingList;
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
                    _cmd.Parameters.AddWithValue("@Turnover", TurnoverText);
                    _cmd.Parameters.AddWithValue("@CompanyReg", CompanyRegText);
                    _cmd.Parameters.AddWithValue("@CompanyType", CompanyType);

                    _cmd.Parameters.AddWithValue("@IDTEXT", IDText);

                    _con.Open();
                    _cmd.ExecuteNonQuery();
                    _con.Close();
                }

                RichTextDatePotential.Document.Blocks.Clear();

                System.Windows.MessageBox.Show("Information Updated!");
                Close();
            }
        }
    

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult dialogResult = (System.Windows.Forms.MessageBox.Show("Are you sure?", "Accept Visit Request", MessageBoxButtons.YesNo));
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                AddVisit();
                DeletePendingVisit();
                System.Windows.MessageBox.Show("Visit Accepted");
            }
            else
            {
                System.Windows.MessageBox.Show("Action Cancelled");
            }
            Close();
        }

        public void VanInfoSQL()
        {
            var ConnectionString = DataAccess.GlobalSQL.ConnectionMySQLVan;
            DataTable VanTableALL = new DataTable();
            var IDRANGE = new TextRange(RichTextVisitID.Document.ContentStart, RichTextVisitID.Document.ContentEnd);
            int IDText = Convert.ToInt32(IDRANGE.Text.ToString().Replace("\r", "").Replace("\n", ""));

            using (MySqlConnection _con = new MySqlConnection(ConnectionString))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.VanListALLPending;

                _con.Open();

                using (MySqlCommand _cmd = new MySqlCommand(queryStatement, _con))
                {
                    MySqlDataAdapter _dap = new MySqlDataAdapter(_cmd);
                    _cmd.Parameters.AddWithValue("@IDTEXT", IDText);
                    _dap.Fill(VanTableALL);
                }
                _con.Close();

                foreach (DataRow Row in VanTableALL.Rows)
                {
                    String COAddress = Row["address"].ToString();
                    RichTextCusAddInfo.AppendText(COAddress);
                    String COPostcode = Row["postcode"].ToString();
                    RichTextPostcode.AppendText(COPostcode);
                    String COTown = Row["town"].ToString();
                    RichTextTown.AppendText(COTown);
                    String ContactName = Row["contact_name"].ToString();
                    RichTextContactName.AppendText(ContactName);
                    String ContactEmail = Row["contact_email"].ToString();
                    RichTextContactEmail.AppendText(ContactEmail);
                    String ContactPhone = Row["contact_phone"].ToString();
                    RichTextContactNum.AppendText(ContactPhone);
                    String VisitDesc = Row["description_collection"].ToString();
                    RichTextVisitDesc.AppendText(VisitDesc);
                    String SalesPerson = Row["sales_person"].ToString();
                    ComboSalesStaff.Text = SalesPerson;
                    String PlannedDate = Row["collection_date"].ToString();
                    if (PlannedDate == "")
                    {
                        RichTextPromDate.AppendText("");
                    }
                    else
                    {
                        RichTextPromDate.AppendText(PlannedDate.Substring(0, 10));
                    }
                    String VisitType = Row["visit_type"].ToString();
                    ComboType.Text = VisitType;
                    String StaffMember = Row["staff_member"].ToString();
                    ComboAdminStaff.Text = StaffMember;
                    String COName = Row["company_name"].ToString();
                    VisitTextBox.Text = COName;
                    String CredChecked = Row["credit_checked"].ToString();
                    ComboCreditChecked.Text = CredChecked;
                    String PlannedStartTime = Row["planned_start"].ToString();
                    ComboPromTime.Text = PlannedStartTime;
                    String PlannedJobTime = Row["job_time"].ToString();
                    ComboJobTime.Text = PlannedJobTime;
                    String AnnualTurnover = Row["annual_spend"].ToString();
                    RichTextTurnover.AppendText(AnnualTurnover);
                    String CompanyReg = Row["company_reg"].ToString();
                    RichTextRegNo.AppendText(CompanyReg);
                    String CompanyType = Row["company_type"].ToString();
                    ComboCompanyType.Text = CompanyType;
                }
            }

        }

        private void AddVisit()
        {
            String COText = VisitTextBox.Text;
            var AddressRange = new TextRange(RichTextCusAddInfo.Document.ContentStart, RichTextCusAddInfo.Document.ContentEnd);
            String AddressText = AddressRange.Text.Replace("\r", "").Replace("\n", "");
            var PostcodeRange = new TextRange(RichTextPostcode.Document.ContentStart, RichTextPostcode.Document.ContentEnd);
            String PostcodeText = PostcodeRange.Text.Replace("\r", "").Replace("\n", "");
            var DescRange = new TextRange(RichTextVisitDesc.Document.ContentStart, RichTextVisitDesc.Document.ContentEnd);
            String DescText = DescRange.Text.Replace("\r", "").Replace("\n", "");
            var NameRange = new TextRange(RichTextContactName.Document.ContentStart, RichTextContactName.Document.ContentEnd);
            String NameText = NameRange.Text.Replace("\r", "").Replace("\n", "");
            var NumberRange = new TextRange(RichTextContactNum.Document.ContentStart, RichTextContactNum.Document.ContentEnd);
            String NumberText = NumberRange.Text.Replace("\r", "").Replace("\n", "");
            var EmailRange = new TextRange(RichTextContactEmail.Document.ContentStart, RichTextContactEmail.Document.ContentEnd);
            String EmailText = EmailRange.Text.Replace("\r", "").Replace("\n", "");
            String SalesText = ComboSalesStaff.Text.ToString();
            String StaffText = ComboAdminStaff.Text.ToString();
            String VisitText = ComboType.Text.ToString();
            var TownRange = new TextRange(RichTextTown.Document.ContentStart, RichTextTown.Document.ContentEnd);
            String TownText = TownRange.Text.Replace("\r", "").Replace("\n", "");
            var IDRANGE = new TextRange(RichTextVisitID.Document.ContentStart, RichTextVisitID.Document.ContentEnd);
            int IDText = Convert.ToInt32(IDRANGE.Text.ToString().Replace("\r", "").Replace("\n", ""));
            var CreditCheckedText = ComboCreditChecked.Text.ToString();
            var PlannedStartText = ComboPromTime.Text.ToString();
            var JobTimeText = ComboJobTime.Text.ToString();
            var CompanyRegRange = new TextRange(RichTextRegNo.Document.ContentStart, RichTextRegNo.Document.ContentEnd);
            String CompanyReg = CompanyRegRange.Text.Replace("\r", "").Replace("\n", "");
            var AnnualTurnoverRange = new TextRange(RichTextTurnover.Document.ContentStart, RichTextTurnover.Document.ContentEnd);
            String AnnualTurnover = AnnualTurnoverRange.Text.Replace("\r", "").Replace("\n", "");
            var CompanyType = ComboCompanyType.Text.ToString();

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
                    _cmd.Parameters.AddWithValue("@IDTEXT", IDText);
                    _cmd.Parameters.AddWithValue("@CreditCheckedText", CreditCheckedText);
                    _cmd.Parameters.AddWithValue("@PlannedStartText", PlannedStartText);
                    _cmd.Parameters.AddWithValue("@JobTimeText", JobTimeText);
                    _cmd.Parameters.AddWithValue("@Turnover", AnnualTurnover);
                    _cmd.Parameters.AddWithValue("@CompanyReg", CompanyReg);
                    _cmd.Parameters.AddWithValue("@CompanyType", CompanyType);

                    _con.Open();
                    _cmd.ExecuteNonQuery();
                    _con.Close();

                }
                RichTextDatePotential.Document.Blocks.Clear();
            }
        }

        private void DeletePendingVisit()
        {
            var IDRANGE = new TextRange(RichTextVisitID.Document.ContentStart, RichTextVisitID.Document.ContentEnd);
            int IDText = Convert.ToInt32(IDRANGE.Text.ToString().Replace("\r", "").Replace("\n", ""));

                var ConnectionString = DataAccess.GlobalSQL.ConnectionMySQLVan;
                using (MySqlConnection _con = new MySqlConnection(ConnectionString))
                {
                    var CommandStatement = DataAccess.GlobalSQLNonQueries.DeleteFromVanPendingList;
                    using (MySqlCommand _cmd = new MySqlCommand(CommandStatement, _con))
                    {

                        _con.Open();
                        _cmd.Parameters.AddWithValue("@IDTEXT", IDText);
                        _cmd.ExecuteNonQuery();
                        _con.Close();
                    }
                }
            

        }

        private void buttonCheckerAccept()
        {
            string loginname = Environment.UserName;
            if (loginname == "MatthewKavanagh" || loginname == "JakeBassi" || loginname == "SophieGroth" || loginname == "AntonyGroth" || loginname == "KylieWoollard")
            {
                BtnAccept.Visibility = Visibility.Visible;
            }

            else
            {
                BtnAccept.Visibility = Visibility.Hidden;
            }
        }

    }
}
