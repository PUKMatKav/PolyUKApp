using Microsoft.Data.SqlClient;
using PolyUKApp.SQL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyUKApp.SQL
{
    class DataAccess
    {
        public List<Item> GetItem(string Code)
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnValue("polysql01")))
            {
                var output = connection.Query<Item> ($"SELECT * from STKStockItemView where Code = '{Code}'").ToList();
                return output;
            }
        }
        public List<ItemList> GetItemList(string SearchSubject, string SearchName)
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnValue("polysql01")))
            {
                var output = connection.Query<ItemList>($"SELECT Code, Name, Description, StockUnitName, ProductGroupDescription " +
                    $"FROM STKStockItemView").ToList();
                return output;
            }
        }

        public List<WONumberList> GetWONumbers(string DateStart)
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnValue("polysql01")))
            {
                var output = connection.Query<WONumberList>($"SELECT * from SiWorksOrderListView WHERE StartDateShort = '{DateStart}'").ToList();
                return output;
            }
        }
        public List<WOInfoDB> GetWOInfo(string WONumber)
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnValue("polysql01")))
            {
                var output = connection.Query<WOInfoDB>($"SELECT * from SiWorksOrderListView WHERE WONumber = '{WONumber}' and (Status = 'New' or Status = 'Allocated' or Status = 'Issued')").ToList();
                return output;
            }
        }
        public List<WOInfoDB2> GetWOInfo2(string WONumber)
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnValue("polysql01")))
            {
                var output = connection.Query<WOInfoDB2>($"SELECT * from SC_BI_Poly_WODetails WHERE WONumber = '{WONumber}' and (WOStatus = 'New' or WOStatus = 'Allocated' or WOStatus = 'Issued')").ToList();
                return output;
            }
        }
        public List<VanList> GetVanList()
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnValue("MySQLVan")))
            {
                var output = connection.Query<VanList>($"SELECT * from collection_database").ToList();
                return output;
            }
        }

        //TEST SERVER CONNECTIONS//
        public List<WOInfoDB> GetWOInfoTEST(string WONumber)
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnValue("polysql01TEST")))
            {
                var output = connection.Query<WOInfoDB>($"SELECT * from SiWorksOrderListView WHERE WONumber = '{WONumber}'").ToList();
                return output;
            }
        }
        public List<WOInfoDB2> GetWOInfo2TEST(string WONumber)
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnValue("polysql01TEST")))
            {
                var output = connection.Query<WOInfoDB2>($"SELECT * from SC_BI_Poly_WODetails WHERE WONumber = '{WONumber}'").ToList();
                return output;
            }
        }


        public static class GlobalSQL
        {
            public static String Connection = (Helper.ConnValue("polysql01")).ToString();
            public static String ConnectionTEST = (Helper.ConnValue("polysql01TEST")).ToString();
            public static String ConnectionMySQLVan = (Helper.ConnValue("MySQLVan")).ToString();
        }

        public static class GlabalSQLQueries
        {
            public static String ItemListQuery = "SELECT Code, Name, Description, StockUnitName AS 'Unit', ProductGroupDescription AS 'Type' " +
                                                 "FROM STKStockItemView " +
                                                 "ORDER BY Code";

            public static String WOItemListQuery = "SELECT Code, Name, Description, StockUnitName AS 'Unit', ProductGroupDescription AS 'Type' " +
                                                 "FROM STKStockItemView " +
                                                 "WHERE Code = @Code";

            public static String WOQuery = "SELECT SiWorksOrderID, WONumber, WOName, Quantity, StartDate, DueDate, Status  " +
                "FROM SiWorksOrderListView " +
                "WHERE (Status = @Status or Status = @Status1 or Status = @Status2)";

            public static String WOQueryEndDate = "SELECT SiWorksOrderID, WONumber, WOName, Quantity, CONVERT(char(10), StartDateShort, 112) as DateStart, CONVERT(char(10), DueDateShort, 112) as DateEnd, Status " +
                "FROM SiWorksOrderListView " +
                "WHERE DueDateShort = @DueDateShort";

            public static String WODetails = "SELECT SiWorksOrderID, WONumber, WOName, WOType, WOStatus, SONumber, CustomerAccountNumber, CustomerAccountName, PromisedDeliveryDate  " +
                "FROM SC_BI_Poly_WODetails " +
                "WHERE (WOStatus = @WOStatus or WOStatus = @WOStatus1 or WOStatus = @WOStatus2)";

            public static String WODetailsList = "SELECT SiWorksOrderID, WONumber, WOType, WOStatus, CustomerAccountName, CONVERT(char(10), PromisedDeliveryDate, 111) AS PromisedDeliveryDate " +
                "FROM SC_BI_Poly_WODetails " +
                "WHERE (WOStatus = @WOStatus or WOStatus = @WOStatus1 or WOStatus = @WOStatus2)";

            public static String WOInfoForList = "SELECT SiWorksOrderID, Quantity, Status  " +
                "FROM SiWorksOrderListView " +
                "WHERE (Status = @Status or Status = @Status1 or Status = @Status2)";

            public static String VanListCombo = "SELECT * from collection_database " +
                "WHERE completed = 'No'";

            public static String VanListDisplayFilter = "SELECT * from [Visits$] " +
                "WHERE [Company Name] =@COName ";

            public static String VanList = "SELECT company_name as 'Company Name', Postcode as Postcode, visit_type as 'Visit Type', collection_date as Date, id as ID " +
                "from collection_database " +
                "WHERE completed = 'No'";

            public static String VanListALL = "SELECT * from collection_database " +
                "WHERE id = @IDTEXT";

            public static String VanListOLD = "SELECT company_name as 'Company Name', address as Address, postcode as Postcode, town as Town, contact_name as 'Contact Name', contact_email as 'Email', contact_phone as 'Phone', description_collection as 'Visit Description', sales_person as 'Sales Person', leads as Leads, collection_date as 'Visited Date', visit_type as 'Visit Type', id as ID, weight_waste as 'Waste collected (kg)' " +
                "FROM collection_database " +
                "WHERE completed = 'Yes'";

            public static String VanListPending = "SELECT company_name as 'Company Name', address as Address, postcode as Postcode, town as Town, contact_name as 'Contact Name', contact_email as 'Email', contact_phone as 'Phone', description_collection as 'Visit Description', sales_person as 'Sales Person', leads as Leads, collection_date as 'Visited Date', visit_type as 'Visit Type', id as ID, weight_waste as 'Waste collected (kg)' " +
                "FROM pending_database ";

            public static String VanListPendingSmall = "SELECT company_name as 'Company Name', Postcode as Postcode, visit_type as 'Visit Type', collection_date as Date, id as ID " +
                "FROM pending_database ";

            public static String VanListALLPending = "SELECT * from pending_database " +
                "WHERE id = @IDTEXT";

        }
        public static class GlobalSQLNonQueries
        {
            public static String UpdateVanList = "UPDATE collection_database " +
                "SET company_name = @COText, town = @TownText, collection_date = @PlannedDate, address = @AddressText, postcode = @PostcodeText, contact_name = @NameText, contact_email = @EmailText, contact_phone = @NumberText, description_collection = @DescText, sales_person = @SalesText, visit_type = @VisitText, staff_member = @StaffText, weight_waste = @WeightText, scrap_type = @WasteTypeText, credit_checked = @CreditCheckedText, planned_start = @PlannedStartText, job_time = @JobTimeText " +
                "WHERE id = @IDTEXT";

            public static String AddVanList = "INSERT INTO collection_database " +
                "SET company_name = @COText, town = @TownText, collection_date = @PlannedDate, address = @AddressText, postcode = @PostcodeText, contact_name = @NameText, contact_email = @EmailText, contact_phone = @NumberText, description_collection = @DescText, sales_person = @SalesText, visit_type = @VisitText, staff_member = @StaffText, id = @IDTEXT, credit_checked = @CreditCheckedText, planned_start = @PlannedStartText, job_time = @JobTimeText, weight_waste = '0', leads = '0', completed = 'No', scrap_type = 'N/A'  ";

            public static String DeleteFromVanList = "DELETE FROM collection_database " +
                "WHERE id = @IDTEXT";

            public static String CompleteFromVanList = "UPDATE collection_database " +
                "SET completed = 'Yes' " +
                "WHERE id = @IDTEXT";

            public static String UNCompleteFromVanList = "UPDATE collection_database " +
                "SET completed = 'No' " +
                "WHERE id = @IDTEXT";

            public static String AddVanRequestList = "INSERT INTO pending_database " +
                "SET company_name = @COText, town = @TownText, collection_date = @PlannedDate, address = @AddressText, postcode = @PostcodeText, contact_name = @NameText, contact_email = @EmailText, contact_phone = @NumberText, description_collection = @DescText, sales_person = @SalesText, visit_type = @VisitText, staff_member = @StaffText, id = @IDTEXT, credit_checked = @CreditCheckedText, planned_start = @PlannedStartText, job_time = @JobTimeText, weight_waste = '0', leads = '0', completed = 'No', scrap_type = 'N/A' ";

            public static String UpdateVanPendingList = "UPDATE pending_database " +
                "SET company_name = @COText, town = @TownText, collection_date = @PlannedDate, address = @AddressText, postcode = @PostcodeText, contact_name = @NameText, contact_email = @EmailText, contact_phone = @NumberText, description_collection = @DescText, sales_person = @SalesText, visit_type = @VisitText, staff_member = @StaffText, credit_checked = @CreditCheckedText, planned_start = @PlannedStartText, job_time = @JobTimeText " +
                "WHERE id = @IDTEXT";

            public static String DeleteFromVanPendingList = "DELETE FROM pending_database " +
                "WHERE id = @IDTEXT";

        }
    }
}
