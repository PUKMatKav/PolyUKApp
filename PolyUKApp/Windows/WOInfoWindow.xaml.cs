using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.Logging;
using PolyUKApp.SQL;
using PolyUKApp.SQL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for WOInfoWindow.xaml
    /// </summary>
    public partial class WOInfoWindow : Window
    {
        List<WOInfoDB> WOInfo = new List<WOInfoDB>();
        List<WOInfoDB2> WOInfo2 = new List<WOInfoDB2>();
        public WOInfoWindow()
        {
            InitializeComponent();
            LoadCode();
            WODatableConnect();
            AdminName();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();

        }
        public void LoadCode()
        {
            string WOnumberCopy = System.Windows.Clipboard.GetText();
            CodeTextBox.Text = WOnumberCopy;
            TextBlockEditDetails.Text = "Edit Details of " + WOnumberCopy;
        }
        private void AdminName()
        {
            TextRange textRange = new TextRange(RichTextSalesPersonInfo.Document.ContentStart, RichTextSalesPersonInfo.Document.ContentEnd);
            if (textRange.Text == "James Woollard\r\n")
            {
                RichTextAdminPersonInfo.AppendText("Monika Klich");
            }
            else if (textRange.Text == "Jack Mungall\r\n" || textRange.Text == "Neerisha Singh\r\n")
            {
                RichTextAdminPersonInfo.AppendText("Kelly Peake");
            }
            else if (textRange.Text == "Tom Matthews\r\n" || textRange.Text == "Ryan King\r\n")
            {
                RichTextAdminPersonInfo.AppendText("Alex Disbrey");
            }
            else if (textRange.Text == "Jason Mayhew\r\n" || textRange.Text == "Natalie Horler\r\n")
            {
                RichTextAdminPersonInfo.AppendText("Maddy Williams");
            }
            else if (textRange.Text == "Donna Rivera\r\n")
            {
                RichTextAdminPersonInfo.AppendText("Donna Rivera");
            }
            else
            {
                RichTextAdminPersonInfo.AppendText("None");
            }
        }

        private async void WODatableConnect()
        {
            DataAccess db = new DataAccess();
            DataAccess db2 = new DataAccess();

            WOInfo = db.GetWOInfo(CodeTextBox.Text);
            if (WOInfo.Count > 0)
            {
                //var Item_name = WOInfo[0].WOName.ToString();
                //var regex_Item_Name = new Regex(Regex.Escape("  "));
                //RichTextItemName.AppendText(regex_Item_Name.Replace(Item_name, "\r", 1));
                var Item_Quantity = Convert.ToDouble(WOInfo[0].Quantity);
                RichTextQtyInfo.AppendText(Item_Quantity.ToString());
                var WO_ID = WOInfo[0].SiWorksOrderID.ToString();
                RichTextWOID.AppendText(WO_ID);

                String StartDay = WOInfo[0].StartDateShort.ToString().Substring(3, 2);
                String StartMonth = WOInfo[0].StartDateShort.ToString().Substring(0, 2);
                String StartYear = WOInfo[0].StartDateShort.ToString().Substring(6, 4);
                RichTextStartDate.AppendText(StartDay + "/" + StartMonth + "/" + StartYear);
                String EndDay = WOInfo[0].DueDateShort.ToString().Substring(3, 2);
                String EndMonth = WOInfo[0].DueDateShort.ToString().Substring(0, 2);
                String EndYear = WOInfo[0].DueDateShort.ToString().Substring(6, 4);
                RichTextEndDate.AppendText(EndDay + "/" + EndMonth + "/" + EndYear);
            }
            else
            {
                TextBlockError.Visibility = Visibility.Visible;
                await Task.Delay(3000);
                TextBlockError.Visibility = Visibility.Hidden;
            }
            WOInfo2 = db2.GetWOInfo2(CodeTextBox.Text);
            if (WOInfo2.Count > 0)
            {
                var Cus_Name = WOInfo2[0].CustomerAccountName.ToString();
                if (Cus_Name == null)
                {
                    RichTextCusNameInfo.AppendText("Blank");
                }
                else
                {
                    RichTextCusNameInfo.AppendText(Cus_Name);
                }
                var SalesPerson = WOInfo2[0].SalesPerson.ToString();
                RichTextSalesPersonInfo.AppendText(SalesPerson);
                var Item_Code = WOInfo2[0].BuiltItem.ToString();
                RichTextItemCode.AppendText(Item_Code);
                var Prom_Date_Day = WOInfo2[0].PromisedDeliveryDate.ToString().Substring(3, 2);
                var Prom_Date_Month = WOInfo2[0].PromisedDeliveryDate.ToString().Substring(0, 2);
                var Prom_Date_Year = WOInfo2[0].PromisedDeliveryDate.ToString().Substring(6, 4);
                RichTextPromDate.AppendText(Prom_Date_Day + "/" + Prom_Date_Month + "/" + Prom_Date_Year);

            }
            else
            {
                TextBlockError.Visibility = Visibility.Visible;
                await Task.Delay(3000);
                TextBlockError.Visibility = Visibility.Hidden;
            }
            var Item_Code_Comparison = WOInfo2[0].BuiltItem.ToString();
            string connectionString = DataAccess.GlobalSQL.Connection;
            DataTable ItemTable = new DataTable("ItemTable");

            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                _con.Open();
                String queryStatement = DataAccess.GlabalSQLQueries.WOItemListQuery;

                using (SqlCommand _cmd = new SqlCommand(queryStatement, _con))
                {
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    _cmd.Parameters.AddWithValue("@Code", Item_Code_Comparison);
                    _dap.Fill(ItemTable);
                }
                foreach (DataRow row in ItemTable.Rows)
                {
                    RichTextItemDesc.AppendText(row["Description"].ToString());
                    if (row["unit"].ToString() == "1000")
                    {
                        RichTextUnitInfo.AppendText(row["Unit"].ToString() + "'s");
                    }
                    else
                    {
                        RichTextUnitInfo.AppendText(row["Unit"].ToString());
                    }

                }

            }

        }

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedstartdate = StartDatePicker.SelectedDate;
            if (selectedstartdate != null)
            {
                RichTextStartDatePotential.Document.Blocks.Clear();
                RichTextStartDatePotential.AppendText("(" + selectedstartdate.ToString().Substring(0, 10) + ")");
            }
            else
            {
                RichTextStartDatePotential.Document.Blocks.Clear();
            }

        }

        private void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedenddate = EndDatePicker.SelectedDate;
            if (selectedenddate != null)
            {
                RichTextEndDatePotential.Document.Blocks.Clear();
                RichTextEndDatePotential.AppendText("(" + selectedenddate.ToString().Substring(0, 10) + ")");
            }
            else
            {
                RichTextEndDatePotential.Document.Blocks.Clear();
            }
        }

        private async void BtnValidate_Click(object sender, RoutedEventArgs e)
        {
            var selectedstartdate = StartDatePicker.SelectedDate;
            var selectedenddate = EndDatePicker.SelectedDate;
            if (selectedstartdate <= selectedenddate)
            {
                TextBlockValidation.Text = "Validated";
                BtnCommit.Visibility = Visibility.Visible;
                TextBlockValidation.Visibility = Visibility.Visible;
                await Task.Delay(3000);
                TextBlockValidation.Visibility = Visibility.Hidden;

            }
            else
            {
                TextBlockValidation.Text = "Start date cannot be after end date";
                BtnCommit.Visibility = Visibility.Hidden;
                TextBlockValidation.Visibility = Visibility.Visible;
                await Task.Delay(3000);
                TextBlockValidation.Visibility = Visibility.Hidden;

            }
        }

        private void BtnCommit_Click(object sender, RoutedEventArgs e)
        {
            TextRange textRangeWOID = new TextRange(RichTextWOID.Document.ContentStart, RichTextWOID.Document.ContentEnd);
            var selectedstartdate = StartDatePicker.SelectedDate;
            var SciConID = textRangeWOID.Text;
            var SciConFInalID = SciConID.ToString().Substring(0, 8);
            string connectionString = DataAccess.GlobalSQL.Connection;
            using (SqlConnection _con = new SqlConnection(connectionString))
            {
                _con.Open();
                String queryStatement = "UPDATE SiWorksOrderListView " +
                                        "SET StartDateShort = '" + selectedstartdate + "' ";
                _con.Execute(queryStatement);


            }
        }
    }
}
