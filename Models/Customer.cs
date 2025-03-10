namespace ConsoleBusinessCentral.Models
{
    public class Customer
    {
        public string No { get; set; } = default!;
        public string? Name { get; set; }
        public string? Name_2 { get; set; }
        public string? Responsibility_Center { get; set; }
        public string? Location_Code { get; set; }
        public string? Post_Code { get; set; }
        public string? Country_Region_Code { get; set; }
        public string? Phone_No { get; set; }
        public string? IC_Partner_Code { get; set; }
        public string? Contact { get; set; }
        public string? Salesperson_Code { get; set; }
        public string? Customer_Posting_Group { get; set; }
        public bool? Allow_Multiple_Posting_Groups { get; set; }
        public string? Gen_Bus_Posting_Group { get; set; }
        public string? VAT_Bus_Posting_Group { get; set; }
        public string? Customer_Price_Group { get; set; }
        public string? Customer_Disc_Group { get; set; }
        public string? Payment_Terms_Code { get; set; }
        public string? Reminder_Terms_Code { get; set; }
        public string? Fin_Charge_Terms_Code { get; set; }
        public string? Currency_Code { get; set; }
        public string? Language_Code { get; set; }
        public string? Search_Name { get; set; }
        public decimal? Credit_Limit_LCY { get; set; }
        public string? Blocked { get; set; }
        public bool? Privacy_Blocked { get; set; }
        public DateTime? Last_Date_Modified { get; set; }
        public string? Application_Method { get; set; }
        public bool? Combine_Shipments { get; set; }
        public string? Reserve { get; set; }
        public string? Ship_to_Code { get; set; }
        public string? Shipping_Advice { get; set; }
        public string? Shipping_Agent_Code { get; set; }
        public string? Base_Calendar_Code { get; set; }
        public decimal? Balance_LCY { get; set; }
        public decimal? Balance_Due_LCY { get; set; }
        public decimal? Sales_LCY { get; set; }
        public decimal? Payments_LCY { get; set; }
        public bool? Coupled_to_CRM { get; set; }
        public bool? Coupled_to_Dataverse { get; set; }
        public string? Global_Dimension_1_Filter { get; set; }
        public string? Global_Dimension_2_Filter { get; set; }
        public string? Currency_Filter { get; set; }
        public string? Date_Filter { get; set; }
    }

}
