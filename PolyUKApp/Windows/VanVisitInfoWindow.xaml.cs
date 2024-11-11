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
using static PolyUKApp.Windows.CallTimeWindow;
using MySql.Data.MySqlClient;
using Dapper;
using IDataObject = System.Windows.IDataObject;
using System.Drawing.Printing;
using Microsoft.VisualBasic.Logging;
using Org.BouncyCastle.Ocsp;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using Org.BouncyCastle.Cms;
using System.Data.SqlTypes;
using System.Net.Mail;
using System.Security.Claims;
using MySqlX.XDevAPI;


namespace PolyUKApp.Windows
{
    /// <summary>
    /// Interaction logic for VanVisitInfoWindow.xaml
    /// </summary>
    public partial class VanVisitInfoWindow : Window
    {
        Bitmap memoryImage;
        DataTable VanListInfo = new DataTable();
        public VanVisitInfoWindow()
        {
            InitializeComponent();
            LoadVisit();
            ComboItems();
            VanInfoSQL();
        }

        public void LoadVisit()
        {
            IDataObject DataID = System.Windows.Clipboard.GetDataObject();
            string VisitID = (String)DataID.GetData(typeof(String));
            RichTextVisitID.AppendText(VisitID);

        }
        /*public void GetVanComboList()
        {
            var connectionString = DataAccess.GlobalSQL.ConnectionMySQLVan;
            DataTable VanList = new DataTable();

            using (MySqlConnection _con = new MySqlConnection(connectionString)) 
            {
                var queryStatement = DataAccess.GlabalSQLQueries.VanListCombo;
                using (MySqlCommand _cmd = new MySqlCommand(queryStatement, _con))
                {
                    MySqlDataAdapter _dap = new MySqlDataAdapter(_cmd);

                    _con.Open();
                    _dap.Fill(VanList);
                    _con.Close();

                }

                List<String> VisitList = new List<String>();
                foreach (DataRow Row in VanList.Rows)
                {
                    VisitList.Add(Row["company_name"].ToString());
                }
                VisitComboBox.ItemsSource = VisitList;

            }
        }*/

        public void ComboItems()
        {
            ComboAdminStaff.Items.Add("Ant");
            ComboAdminStaff.Items.Add("Jake");
            ComboAdminStaff.Items.Add("Both");

            ComboType.Items.Add("Technical");
            ComboType.Items.Add("Collection");
            ComboType.Items.Add("Delivery");
            ComboType.Items.Add("Part Collected");
            ComboType.Items.Add("Other");

            ComboSalesStaff.Items.Add("Donna Rivera");
            ComboSalesStaff.Items.Add("Jack Mungall");
            ComboSalesStaff.Items.Add("James Woollard");
            ComboSalesStaff.Items.Add("Jason Mayhew");
            ComboSalesStaff.Items.Add("Natalie Horler");
            ComboSalesStaff.Items.Add("Neerisha Singh");
            ComboSalesStaff.Items.Add("Ryan King");
            ComboSalesStaff.Items.Add("Tom Matthews");

            ComboCollectedType.Items.Add("N/A");
            ComboCollectedType.Items.Add("Clear");
            ComboCollectedType.Items.Add("Jazz");
            ComboCollectedType.Items.Add("Mixed");

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

        /*public void VisitInfoContent()
        {
            string CurrentUser = Globals.Username;
            string filepath = "C:\\Users\\" + CurrentUser + "\\Polythene UK Limited\\Shared - Documents\\Waste Collection\\2024 Collection List Database.xlsx;";
            string conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + @"Extended Properties='Excel 8.0;HDR=Yes;'";
            string CONameText = VisitComboBox.Text.ToString();

            using (OleDbConnection _con = new OleDbConnection(conn))
            {
                _con.Open();
                using (OleDbCommand _cmd = new OleDbCommand(DataAccess.GlabalSQLQueries.VanListDisplayFilter, _con))
                {
                    OleDbDataAdapter _dap = new OleDbDataAdapter( _cmd);
                    _cmd.Parameters.AddWithValue("@COName", CONameText);
                    _dap.Fill(VanListInfo);
                }
                _con.Close();

                foreach(DataRow Row in VanListInfo.Rows)
                {
                    String COAddress = Row["Address"].ToString();
                    RichTextCusAddInfo.AppendText(COAddress);
                    String COPostcode = Row["Postcode"].ToString();
                    RichTextPostcode.AppendText(COPostcode);
                    String COTown = Row["Town"].ToString();
                    RichTextTown.AppendText(COTown);
                    String ContactName = Row["Contact Name"].ToString();
                    RichTextContactName.AppendText(ContactName);
                    String ContactEmail = Row["Contact Email"].ToString();
                    RichTextContactEmail.AppendText(ContactEmail);
                    String ContactPhone = Row["Contact Phone"].ToString();
                    RichTextContactNum.AppendText(ContactPhone);
                    String VisitDesc = Row["Description of Collection"].ToString();
                    RichTextVisitDesc.AppendText(VisitDesc);
                    String SalesPerson = Row["Sales Person"].ToString();
                    RichTextSalesPersonInfo.AppendText(SalesPerson);
                    String PlannedDate = Row["Planned Collection Date"].ToString();
                    if(PlannedDate == "")
                    {
                        RichTextPromDate.AppendText("Not Set");
                    }
                    else
                    {
                        RichTextPromDate.AppendText(PlannedDate.Substring(0,10));
                    }
                    String VisitType = Row["Visit Type"].ToString();
                    ComboType.Text = VisitType;
                    String StaffMember = Row["Staff Member"].ToString();
                    ComboAdminStaff.Text = StaffMember;
                    String VisitID = Row["ID"].ToString();
                    RichTextVisitID.AppendText(VisitID);

                }


            }
        }*/

        public void VanInfoSQL()
        {
            var ConnectionString = DataAccess.GlobalSQL.ConnectionMySQLVan;
            DataTable VanTableALL = new DataTable();
            var IDRANGE = new TextRange(RichTextVisitID.Document.ContentStart, RichTextVisitID.Document.ContentEnd);
            int IDText = Convert.ToInt32(IDRANGE.Text.ToString().Replace("\r","").Replace("\n", ""));

            using (MySqlConnection _con = new MySqlConnection(ConnectionString))
            {
                var queryStatement = DataAccess.GlabalSQLQueries.VanListALL;

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
                    String WeightCollected = Row["weight_waste"].ToString();
                    RichTextWeight.AppendText(WeightCollected);
                    String StaffMember = Row["staff_member"].ToString();
                    ComboAdminStaff.Text = StaffMember;
                    String COName = Row["company_name"].ToString();
                    VisitTextBox.Text = COName;
                    String WasteType = Row["scrap_type"].ToString();
                    ComboCollectedType.Text = WasteType;
                    String CredChecked = Row["credit_checked"].ToString();
                    ComboCreditChecked.Text = CredChecked;
                    String PlannedStartTime = Row["planned_start"].ToString();
                    ComboPromTime.Text = PlannedStartTime;
                    String PlannedJobTime = Row["job_time"].ToString();
                    ComboJobTime.Text = PlannedJobTime;
                }
            }

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDate = StartDatePicker.SelectedDate;
            if (selectedDate != null)
            {
                RichTextPromDate.Document.Blocks.Clear();
                RichTextPromDate.AppendText(selectedDate.ToString().Substring(0,10));
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
            var WeightRange = new TextRange(RichTextWeight.Document.ContentStart, RichTextWeight.Document.ContentEnd);
            int WeightText = Convert.ToInt32(WeightRange.Text.Replace("\r", "").Replace("\n", ""));
            var TownRange = new TextRange(RichTextTown.Document.ContentStart, RichTextTown.Document.ContentEnd);
            String TownText = TownRange.Text.Replace("\r", "").Replace("\n", "");
            var IDRANGE = new TextRange(RichTextVisitID.Document.ContentStart, RichTextVisitID.Document.ContentEnd);
            int IDText = Convert.ToInt32(IDRANGE.Text.ToString().Replace("\r", "").Replace("\n", ""));
            var WasteTypeText = ComboCollectedType.Text.ToString();
            var CreditCheckedText = ComboCreditChecked.Text.ToString();
            var PlannedStartText = ComboPromTime.Text.ToString();
            var JobTimeText = ComboJobTime.Text.ToString();

            string connectionString = DataAccess.GlobalSQL.ConnectionMySQLVan;

            using (MySqlConnection _con = new MySqlConnection(connectionString))
            {
                
                var CommandStatement = DataAccess.GlobalSQLNonQueries.UpdateVanList;
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
                    _cmd.Parameters.AddWithValue("@WeightText", WeightText);
                    _cmd.Parameters.AddWithValue("@PlannedDate", DateText);
                    _cmd.Parameters.AddWithValue("@COText", COText);
                    _cmd.Parameters.AddWithValue("@WasteTypeText", WasteTypeText);
                    _cmd.Parameters.AddWithValue("@CreditCheckedText", CreditCheckedText);
                    _cmd.Parameters.AddWithValue("@PlannedStartText", PlannedStartText);
                    _cmd.Parameters.AddWithValue("@JobTimeText", JobTimeText);

                    _cmd.Parameters.AddWithValue("@IDTEXT", IDText);

                    _con.Open();
                    _cmd.ExecuteNonQuery();
                    _con.Close();
                }
                

                RichTextDatePotential.Document.Blocks.Clear();

                /*foreach (DataRow row in VanListInfo.Rows)
                {
                    object VisitID = row["ID"];
                    object COAddress = row["Address"];
                    object COName = row["Company Name"];
                    object COTown = row["Town"];
                    object COPostcode = row["Postcode"];
                    var COStaff = row["Staff Member"];

                    TextRange PlannedDateRange = new TextRange(RichTextPromDate.Document.ContentStart, RichTextPromDate.Document.ContentEnd);
                    String PlannedDateRangeString = (PlannedDateRange.Text).Replace("\r", "").Replace("\n", "");


                    
                    object StaffText = ComboAdminStaff.Text;
                    object TypeText = ComboType.Text;

                    if (PlannedDateRangeString != "Not Set")
                    {
                        int PlannedDay = Convert.ToInt32(PlannedDateRangeString.ToString().Substring(0, 2));
                        int PlannedMonth = Convert.ToInt32(PlannedDateRangeString.ToString().Substring(3, 2));
                        int PlannedYR = Convert.ToInt32(PlannedDateRangeString.ToString().Substring(6, 4));
                        DateTime PlannedDateTime = new DateTime(PlannedYR, PlannedMonth, PlannedDay);

                        using (OleDbCommand _cmd = new OleDbCommand("UPDATE [Visits$] " +
                            "SET [Staff Member]=@SM, " +
                            "[Visit Type]=@VT, " +
                            "[Planned Collection Date]=@PCD " +
                            "WHERE [ID]=@VisitID", _con))
                        {
                            _cmd.Parameters.AddWithValue("@SM", StaffText.ToString());
                            _cmd.Parameters.AddWithValue("@VT", TypeText.ToString());
                            _cmd.Parameters.AddWithValue("@PCD", PlannedDateTime);
                            _cmd.Parameters.AddWithValue("@VisitID", Convert.ToInt32(VisitID));
                            _cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (OleDbCommand _cmd = new OleDbCommand("UPDATE [Visits$] " +
                            "SET [Staff Member]=@SM, " +
                            "[Visit Type]=@VT " +
                            "WHERE [ID]=@VisitID", _con))
                        {
                            _cmd.Parameters.AddWithValue("@SM", StaffText);
                            _cmd.Parameters.AddWithValue("@VT", TypeText.ToString());
                            _cmd.Parameters.AddWithValue("@VisitID", VisitID.ToString());
                            _cmd.ExecuteNonQuery();
                        }
                    }

                }*/
                System.Windows.MessageBox.Show("Information Updated!");
                Close();
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.PrintDialog dialog = new System.Windows.Controls.PrintDialog();
            dialog.ShowDialog();
            dialog.PrintVisual(GridDetails, "Info Grid");
        }

        private void BtnAppoint_Click(object sender, RoutedEventArgs e)
        {
            var DateRange = new TextRange(RichTextPromDate.Document.ContentStart, RichTextPromDate.Document.ContentEnd);
            String DateText = DateRange.Text.Replace("\r", "").Replace("\n", "");
            var VanStartTime = ComboPromTime.Text;
            var VanJobTime = ComboJobTime.Text;

            if (DateText == "")
            {
                System.Windows.MessageBox.Show("Please Enter Date to send calendar invite");
            }
            else if (VanStartTime == "")
            {
                System.Windows.MessageBox.Show("Please Enter Start Time to send calendar invite");
            }
            else if (VanJobTime == "")
            {
                System.Windows.MessageBox.Show("Please Enter Job Time to send calendar invite");
            }
            else
            {
                String StaffText = ComboAdminStaff.Text.ToString();
                if (StaffText == "Jake")
                {
                    SendICalJake();
                }
                else if (StaffText == "Ant")
                {
                    SendICalAnt();
                }
                else
                {
                    SendICalJake();
                    SendICalAnt();
                }
                
            }
        }

        private void SendICalJake()
        {
            String COText = VisitTextBox.Text;
            var AddressRange = new TextRange(RichTextCusAddInfo.Document.ContentStart, RichTextCusAddInfo.Document.ContentEnd);
            String AddressText = AddressRange.Text.Replace("\r", "").Replace("\n", "");
            var TownRange = new TextRange(RichTextTown.Document.ContentStart, RichTextTown.Document.ContentEnd);
            String TownText = TownRange.Text.Replace("\r", "").Replace("\n", "");
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
            String StaffText = ComboAdminStaff.Text.ToString();

            var DateRange = new TextRange(RichTextPromDate.Document.ContentStart, RichTextPromDate.Document.ContentEnd);
            String DateText = DateRange.Text.Replace("\r", "").Replace("\n", "");
            object VanStartDay = DateText.Substring(0, 2);
            object VanStartMonth = DateText.Substring(3, 2);
            object VanStartYr = DateText.Substring(6, 4);
            object VanStartTimeHr = ComboPromTime.Text.ToString().Substring(0,2);
            object VanStartTimeMin = ComboPromTime.Text.ToString().Substring(3,2);
            object VanJobTime = ComboJobTime.Text.ToString();
            
            var SysDateStart = new DateTime(Convert.ToInt32(VanStartYr), Convert.ToInt32(VanStartMonth), Convert.ToInt32(VanStartDay), Convert.ToInt32(VanStartTimeHr), Convert.ToInt32(VanStartTimeMin), 00);
            var SysDateEnd = SysDateStart.AddHours(Convert.ToSingle(VanJobTime));

            string _sender = "matthewkavanagh@polytheneuk.co.uk";
            string _password = "Yos55527";

            string startTime1 = Convert.ToDateTime(SysDateStart).ToString("yyyyMMddTHHmmssZ");
            string endTime1 = Convert.ToDateTime(SysDateEnd).ToString("yyyyMMddTHHmmssZ");
            SmtpClient sc = new SmtpClient("polytheneuk.mail.protection.outlook.com");
            sc.Port = 25;
            System.Net.NetworkCredential credentials =
            new System.Net.NetworkCredential(_sender, _password);
            sc.EnableSsl = true;
            sc.Credentials = credentials;


            MailMessage msg = new MailMessage();

            msg.From = new MailAddress("matthewkavanagh@polytheneuk.co.uk", "Van Visit");
            msg.To.Add(new MailAddress("jakebassi@polytheneuk.co.uk"));
            msg.Subject = "Visit to " + COText;
            msg.Body = "Address: " + AddressText + ", " + TownText + ", " + PostcodeText + "\v" + "Job: " + DescText + "\v" + "Contact: " + NameText + " - " + NumberText + " - " + EmailText;

            StringBuilder str = new StringBuilder();
            str.AppendLine("BEGIN:VCALENDAR");

            //PRODID: identifier for the product that created the Calendar object
            str.AppendLine("PRODID:-//ABC Company//Outlook MIMEDIR//EN");
            str.AppendLine("VERSION:2.0");
            str.AppendLine("METHOD:REQUEST");

            str.AppendLine("BEGIN:VEVENT");

            str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", startTime1));//TimeZoneInfo.ConvertTimeToUtc("BeginTime").ToString("yyyyMMddTHHmmssZ")));
            str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow));
            str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", endTime1));//TimeZoneInfo.ConvertTimeToUtc("EndTime").ToString("yyyyMMddTHHmmssZ")));
            str.AppendLine(string.Format("LOCATION: {0}", PostcodeText));

            // UID should be unique.
            str.AppendLine(string.Format("UID:{0}", Guid.NewGuid()));
            str.AppendLine(string.Format("DESCRIPTION:{0}", msg.Body));
            str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", msg.Body));
            str.AppendLine(string.Format("SUMMARY:{0}", msg.Subject));

            str.AppendLine("STATUS:CONFIRMED");
            str.AppendLine("BEGIN:VALARM");
            str.AppendLine("TRIGGER:-PT15M");
            str.AppendLine("ACTION:Accept");
            str.AppendLine("DESCRIPTION:Reminder");
            str.AppendLine("X-MICROSOFT-CDO-BUSYSTATUS:BUSY");
            str.AppendLine("END:VALARM");
            str.AppendLine("END:VEVENT");

            str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", msg.From.Address));
            str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", msg.To[0].DisplayName, msg.To[0].Address));

            str.AppendLine("END:VCALENDAR");
            System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType("text/calendar");
            ct.Parameters.Add("method", "REQUEST");
            ct.Parameters.Add("name", "meeting.ics");
            AlternateView avCal = AlternateView.CreateAlternateViewFromString(str.ToString(), ct);
            msg.AlternateViews.Add(avCal);
            //Response.Write(str);
            // sc.ServicePoint.MaxIdleTime = 2;
            sc.Send(msg);
        }

        private void SendICalAnt()
        {
            String COText = VisitTextBox.Text;
            var AddressRange = new TextRange(RichTextCusAddInfo.Document.ContentStart, RichTextCusAddInfo.Document.ContentEnd);
            String AddressText = AddressRange.Text.Replace("\r", "").Replace("\n", "");
            var TownRange = new TextRange(RichTextTown.Document.ContentStart, RichTextTown.Document.ContentEnd);
            String TownText = TownRange.Text.Replace("\r", "").Replace("\n", "");
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
            String StaffText = ComboAdminStaff.Text.ToString();

            var DateRange = new TextRange(RichTextPromDate.Document.ContentStart, RichTextPromDate.Document.ContentEnd);
            String DateText = DateRange.Text.Replace("\r", "").Replace("\n", "");
            object VanStartDay = DateText.Substring(0, 2);
            object VanStartMonth = DateText.Substring(3, 2);
            object VanStartYr = DateText.Substring(6, 4);
            var SysDateStart = new DateTime(Convert.ToInt32(VanStartYr), Convert.ToInt32(VanStartMonth), Convert.ToInt32(VanStartDay), 08, 30, 00);
            var SysDateEnd = SysDateStart.AddHours(7.5);

            string _sender = "matthewkavanagh@polytheneuk.co.uk";
            string _password = "Yos55527";

            string startTime1 = Convert.ToDateTime(SysDateStart).ToString("yyyyMMddTHHmmssZ");
            string endTime1 = Convert.ToDateTime(SysDateEnd).ToString("yyyyMMddTHHmmssZ");
            SmtpClient sc = new SmtpClient("polytheneuk.mail.protection.outlook.com");
            sc.Port = 25;
            System.Net.NetworkCredential credentials =
            new System.Net.NetworkCredential(_sender, _password);
            sc.EnableSsl = true;
            sc.Credentials = credentials;


            MailMessage msg = new MailMessage();

            msg.From = new MailAddress("matthewkavanagh@polytheneuk.co.uk", "Van Visit");
            msg.To.Add(new MailAddress("antonygroth@polytheneuk.co.uk"));
            msg.Subject = "Visit to " + COText;
            msg.Body = "Address: " + AddressText + ", " + TownText + ", " + PostcodeText + "\v" + "Job: " + DescText + "\v" + "Contact: " + NameText + " - " + NumberText + " - " + EmailText;

            StringBuilder str = new StringBuilder();
            str.AppendLine("BEGIN:VCALENDAR");

            //PRODID: identifier for the product that created the Calendar object
            str.AppendLine("PRODID:-//ABC Company//Outlook MIMEDIR//EN");
            str.AppendLine("VERSION:2.0");
            str.AppendLine("METHOD:REQUEST");

            str.AppendLine("BEGIN:VEVENT");

            str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", startTime1));//TimeZoneInfo.ConvertTimeToUtc("BeginTime").ToString("yyyyMMddTHHmmssZ")));
            str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow));
            str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", endTime1));//TimeZoneInfo.ConvertTimeToUtc("EndTime").ToString("yyyyMMddTHHmmssZ")));
            str.AppendLine(string.Format("LOCATION: {0}", PostcodeText));

            // UID should be unique.
            str.AppendLine(string.Format("UID:{0}", Guid.NewGuid()));
            str.AppendLine(string.Format("DESCRIPTION:{0}", msg.Body));
            str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", msg.Body));
            str.AppendLine(string.Format("SUMMARY:{0}", msg.Subject));

            str.AppendLine("STATUS:CONFIRMED");
            str.AppendLine("BEGIN:VALARM");
            str.AppendLine("TRIGGER:-PT15M");
            str.AppendLine("ACTION:Accept");
            str.AppendLine("DESCRIPTION:Reminder");
            str.AppendLine("X-MICROSOFT-CDO-BUSYSTATUS:BUSY");
            str.AppendLine("END:VALARM");
            str.AppendLine("END:VEVENT");

            str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", msg.From.Address));
            str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", msg.To[0].DisplayName, msg.To[0].Address));

            str.AppendLine("END:VCALENDAR");
            System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType("text/calendar");
            ct.Parameters.Add("method", "REQUEST");
            ct.Parameters.Add("name", "meeting.ics");
            AlternateView avCal = AlternateView.CreateAlternateViewFromString(str.ToString(), ct);
            msg.AlternateViews.Add(avCal);
            //Response.Write(str);
            // sc.ServicePoint.MaxIdleTime = 2;
            sc.Send(msg);
        }

        private void BtnRemoveDate_Click(object sender, RoutedEventArgs e)
        {
            RichTextPromDate.Document.Blocks.Clear();
        }
    }
}
