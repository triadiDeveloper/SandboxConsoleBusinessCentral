using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace ConsoleBusinessCentral.Extensions
{
    public class ConfigureDataCustomer
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly RestSharpHelper _restSharpHelper;

        public ConfigureDataCustomer(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(60) };
            _restSharpHelper = new RestSharpHelper(configuration);
        }

        public async Task FetchCustomersAsync()
        {
            const string urlBase = "https://api.businesscentral.dynamics.com/v2.0/11560066-52c2-4a64-87e5-60e371425dd5/TRAIN/ODataV4/Company('Adhi%20Cakra%20Utama%20Mulia')/Customers";
            const int pageSize = 1000;

            try
            {
                // Get customer data
                var customers = await _restSharpHelper.GetDataAsync(urlBase, pageSize);

                if (customers != null && customers.Any())
                {
                    await BulkInsertCustomersIntoDatabase(customers);
                }
                else
                {
                    Console.WriteLine("No customer data retrieved.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching customer data: {ex.Message}");
            }
        }

        private async Task BulkInsertCustomersIntoDatabase(List<dynamic> customers)
        {
            string connectionString = _configuration.GetConnectionString("DEV");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            // Start measuring time
            var stopwatch = Stopwatch.StartNew();

            // Convert dynamic list to DataTable
            var customerTable = ConvertToDataTable(customers);

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepIdentity, transaction))
                        {
                            bulkCopy.DestinationTableName = "Customers";
                            bulkCopy.BatchSize = 10000;  // Increase batch size for better performance
                            bulkCopy.BulkCopyTimeout = 600; // Increase timeout to avoid timeouts for large data

                            // Optimize column mappings using loop for cleaner code
                            var columnMappings = new Dictionary<string, string>
                    {
                        { "No", "No" }, { "Name", "Name" }, { "Name_2", "Name_2" },
                        { "Responsibility_Center", "Responsibility_Center" }, { "Location_Code", "Location_Code" },
                        { "Post_Code", "Post_Code" }, { "Country_Region_Code", "Country_Region_Code" },
                        { "Phone_No", "Phone_No" }, { "IC_Partner_Code", "IC_Partner_Code" },
                        { "Contact", "Contact" }, { "Salesperson_Code", "Salesperson_Code" },
                        { "Customer_Posting_Group", "Customer_Posting_Group" },
                        { "Allow_Multiple_Posting_Groups", "Allow_Multiple_Posting_Groups" },
                        { "Gen_Bus_Posting_Group", "Gen_Bus_Posting_Group" },
                        { "VAT_Bus_Posting_Group", "VAT_Bus_Posting_Group" },
                        { "Customer_Price_Group", "Customer_Price_Group" },
                        { "Customer_Disc_Group", "Customer_Disc_Group" },
                        { "Payment_Terms_Code", "Payment_Terms_Code" },
                        { "Reminder_Terms_Code", "Reminder_Terms_Code" },
                        { "Fin_Charge_Terms_Code", "Fin_Charge_Terms_Code" },
                        { "Currency_Code", "Currency_Code" },
                        { "Language_Code", "Language_Code" },
                        { "Search_Name", "Search_Name" },
                        { "Credit_Limit_LCY", "Credit_Limit_LCY" },
                        { "Blocked", "Blocked" },
                        { "Privacy_Blocked", "Privacy_Blocked" },
                        { "Last_Date_Modified", "Last_Date_Modified" },
                        { "Application_Method", "Application_Method" },
                        { "Combine_Shipments", "Combine_Shipments" },
                        { "Reserve", "Reserve" },
                        { "Ship_to_Code", "Ship_to_Code" },
                        { "Shipping_Advice", "Shipping_Advice" },
                        { "Shipping_Agent_Code", "Shipping_Agent_Code" },
                        { "Base_Calendar_Code", "Base_Calendar_Code" },
                        { "Balance_LCY", "Balance_LCY" },
                        { "Balance_Due_LCY", "Balance_Due_LCY" },
                        { "Sales_LCY", "Sales_LCY" },
                        { "Payments_LCY", "Payments_LCY" },
                        { "Coupled_to_CRM", "Coupled_to_CRM" },
                        { "Coupled_to_Dataverse", "Coupled_to_Dataverse" },
                        { "Global_Dimension_1_Filter", "Global_Dimension_1_Filter" },
                        { "Global_Dimension_2_Filter", "Global_Dimension_2_Filter" },
                        { "Currency_Filter", "Currency_Filter" },
                        { "Date_Filter", "Date_Filter" }
                    };

                            foreach (var mapping in columnMappings)
                            {
                                bulkCopy.ColumnMappings.Add(mapping.Key, mapping.Value);
                            }

                            // Execute bulk copy operation
                            await bulkCopy.WriteToServerAsync(customerTable);
                        }

                        await transaction.CommitAsync();
                        Console.WriteLine($"{customers.Count} customers inserted successfully.");
                        // Stop the stopwatch and print elapsed time
                        stopwatch.Stop();
                        Console.WriteLine($"Total Get Data Completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"Error during bulk insert: {ex.Message}");
                    }
                }
            }
        }

        private DataTable ConvertToDataTable(List<dynamic> customers)
        {
            var table = new DataTable();

            // Add columns to the DataTable to match the database schema exactly
            table.Columns.AddRange(new DataColumn[]
            {
        new DataColumn("No", typeof(string)),
        new DataColumn("Name", typeof(string)),
        new DataColumn("Name_2", typeof(string)),
        new DataColumn("Responsibility_Center", typeof(string)),
        new DataColumn("Location_Code", typeof(string)),
        new DataColumn("Post_Code", typeof(string)),
        new DataColumn("Country_Region_Code", typeof(string)),
        new DataColumn("Phone_No", typeof(string)),
        new DataColumn("IC_Partner_Code", typeof(string)),
        new DataColumn("Contact", typeof(string)),
        new DataColumn("Salesperson_Code", typeof(string)),
        new DataColumn("Customer_Posting_Group", typeof(string)),
        new DataColumn("Allow_Multiple_Posting_Groups", typeof(bool)),
        new DataColumn("Gen_Bus_Posting_Group", typeof(string)),
        new DataColumn("VAT_Bus_Posting_Group", typeof(string)),
        new DataColumn("Customer_Price_Group", typeof(string)),
        new DataColumn("Customer_Disc_Group", typeof(string)),
        new DataColumn("Payment_Terms_Code", typeof(string)),
        new DataColumn("Reminder_Terms_Code", typeof(string)),
        new DataColumn("Fin_Charge_Terms_Code", typeof(string)),
        new DataColumn("Currency_Code", typeof(string)),
        new DataColumn("Language_Code", typeof(string)),
        new DataColumn("Search_Name", typeof(string)),
        new DataColumn("Credit_Limit_LCY", typeof(decimal)),
        new DataColumn("Blocked", typeof(bool)),
        new DataColumn("Privacy_Blocked", typeof(bool)),
        new DataColumn("Last_Date_Modified", typeof(DateTime)),
        new DataColumn("Application_Method", typeof(string)),
        new DataColumn("Combine_Shipments", typeof(bool)),
        new DataColumn("Reserve", typeof(string)),
        new DataColumn("Ship_to_Code", typeof(string)),
        new DataColumn("Shipping_Advice", typeof(string)),
        new DataColumn("Shipping_Agent_Code", typeof(string)),
        new DataColumn("Base_Calendar_Code", typeof(string)),
        new DataColumn("Balance_LCY", typeof(decimal)),
        new DataColumn("Balance_Due_LCY", typeof(decimal)),
        new DataColumn("Sales_LCY", typeof(decimal)),
        new DataColumn("Payments_LCY", typeof(decimal)),
        new DataColumn("Coupled_to_CRM", typeof(bool)),
        new DataColumn("Coupled_to_Dataverse", typeof(bool)),
        new DataColumn("Global_Dimension_1_Filter", typeof(string)),
        new DataColumn("Global_Dimension_2_Filter", typeof(string)),
        new DataColumn("Currency_Filter", typeof(string)),
        new DataColumn("Date_Filter", typeof(string))
            });

            // Add rows to the DataTable with proper value conversions
            customers.ForEach(customer =>
            {
                table.Rows.Add(
                    customer?.No ?? DBNull.Value,
                    customer?.Name ?? DBNull.Value,
                    customer?.Name_2 ?? DBNull.Value,
                    customer?.Responsibility_Center ?? DBNull.Value,
                    customer?.Location_Code ?? DBNull.Value,
                    customer?.Post_Code ?? DBNull.Value,
                    customer?.Country_Region_Code ?? DBNull.Value,
                    customer?.Phone_No ?? DBNull.Value,
                    customer?.IC_Partner_Code ?? DBNull.Value,
                    customer?.Contact ?? DBNull.Value,
                    customer?.Salesperson_Code ?? DBNull.Value,
                    customer?.Customer_Posting_Group ?? DBNull.Value,
                    ConvertToBoolean(customer?.Allow_Multiple_Posting_Groups),
                    customer?.Gen_Bus_Posting_Group ?? DBNull.Value,
                    customer?.VAT_Bus_Posting_Group ?? DBNull.Value,
                    customer?.Customer_Price_Group ?? DBNull.Value,
                    customer?.Customer_Disc_Group ?? DBNull.Value,
                    customer?.Payment_Terms_Code ?? DBNull.Value,
                    customer?.Reminder_Terms_Code ?? DBNull.Value,
                    customer?.Fin_Charge_Terms_Code ?? DBNull.Value,
                    customer?.Currency_Code ?? DBNull.Value,
                    customer?.Language_Code ?? DBNull.Value,
                    customer?.Search_Name ?? DBNull.Value,
                    ConvertToDecimal(customer?.Credit_Limit_LCY),
                    ConvertToBoolean(customer?.Blocked),
                    ConvertToBoolean(customer?.Privacy_Blocked),
                    ConvertToDateTime(customer?.Last_Date_Modified),
                    customer?.Application_Method ?? DBNull.Value,
                    ConvertToBoolean(customer?.Combine_Shipments),
                    customer?.Reserve ?? DBNull.Value,
                    customer?.Ship_to_Code ?? DBNull.Value,
                    customer?.Shipping_Advice ?? DBNull.Value,
                    customer?.Shipping_Agent_Code ?? DBNull.Value,
                    customer?.Base_Calendar_Code ?? DBNull.Value,
                    ConvertToDecimal(customer?.Balance_LCY),
                    ConvertToDecimal(customer?.Balance_Due_LCY),
                    ConvertToDecimal(customer?.Sales_LCY),
                    ConvertToDecimal(customer?.Payments_LCY),
                    ConvertToBoolean(customer?.Coupled_to_CRM),
                    ConvertToBoolean(customer?.Coupled_to_Dataverse),
                    customer?.Global_Dimension_1_Filter ?? DBNull.Value,
                    customer?.Global_Dimension_2_Filter ?? DBNull.Value,
                    customer?.Currency_Filter ?? DBNull.Value,
                    customer?.Date_Filter ?? DBNull.Value
                );
            });

            return table;
        }

        private bool ConvertToBoolean(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;  // Default value when empty or null

            if (bool.TryParse(value.ToString(), out bool result))
                return result;

            return false;  // Default value if parsing fails
        }

        private decimal ConvertToDecimal(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return 0;  // Default value for empty or null

            if (decimal.TryParse(value.ToString(), out decimal result))
                return result;

            return 0;  // Default value if parsing fails
        }

        private DateTime ConvertToDateTime(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return DateTime.MinValue;  // Default date

            if (DateTime.TryParse(value.ToString(), out DateTime result))
                return result;

            return DateTime.MinValue;  // Default date if parsing fails
        }

    }
}