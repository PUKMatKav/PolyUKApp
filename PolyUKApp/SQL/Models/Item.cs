using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyUKApp.SQL.Models
{
    public class Item
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StockUnitName { get; set; }
        public string ProductGroupDescription { get; set; }
        public string Weight { get; set; }
        public string FreeStockQuantity { get; set; }
        public string AverageBuyingPrice { get; set; }

        public string ItemCodeLength
        {
            get
            {
                return $"{Code}";
            }
        }
    }
    public class ItemList
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [DisplayName("Unit")]
        public string StockUnitName { get; set; }

        [DisplayName("Type")]
        public string ProductGroupDescription { get; set; }

    }

    public class WONumberList
    {
        public string WONumber { get; set; }
        public string WOName { get; set; }
        public string Quantity { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Status { get; set; }
    }

    public class WOInfoDB
    {
        public string WONumber { get; set; }
        public string WOName { get; set; }
        public string Quantity { get; set; }
        public string StartDateShort { get; set; }
        public string DueDateShort { get; set; }
        public string Status { get; set; }
        public string SiWorksOrderID { get; set; }
    }
    public class WOInfoDB2
    {
        public string WONumber { get; set; }
        public string WOName { get; set; }
        public string CustomerAccountNumber { get; set; }
        public string CustomerAccountName { get; set; }
        public string SalesPerson { get; set; }
        public string BuiltItem { get; set; }
        public string PromisedDeliveryDate { get; set; }
        public string SiWorksOrderID { get; set; }
    }

    public class VanList
    {
        [DisplayName("Company Name")]
        public string company_name { get; set; }
        [DisplayName("Address")]
        public string address { get; set; }
        [DisplayName("Postcode")]
        public string postcode { get; set; }
        [DisplayName("Contact Name")]
        public string contact_name { get; set; }
        [DisplayName("Contact Email")]
        public string contact_email { get; set; }
        [DisplayName("Contact Phone")]
        public string contact_phone { get; set; }
        [DisplayName("Description of Visit")]
        public string description_collection { get; set; }
        [DisplayName("Sales Person")]
        public string sales_person { get; set; }
        [DisplayName("Visit Form")]
        public DateTime visit_form { get; set; }
        [DisplayName("Waste Form")]
        public DateTime waste_form { get; set; }
        [DisplayName("Leads")]
        public int leads {  get; set; }
        [DisplayName("Planned Collection Date")]
        public DateTime collection_date { get; set; }
        [DisplayName("Visit Type")]
        public string visit_type { get; set; }
        [DisplayName("Staff Member")]
        public string staff_member {  get; set; }
        [DisplayName("ID")]
        public double id { get; set; }
        [DisplayName("Weight of Waste")]
        public int weight_waste { get; set; }
        [DisplayName("Completed")]
        public string completed {  get; set; }

    }
}



