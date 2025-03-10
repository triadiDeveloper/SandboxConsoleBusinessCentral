using ConsoleBusinessCentral.Models;
using ConsoleBusinessCentral.Persistence;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace ConsoleBusinessCentral.Service
{
    public class ConfigureDataCustomer
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly RestSharpHelper _restSharpHelper;
        private readonly ConsoleDbContext _dbContext;  // DbContext sebagai properti global

        public ConfigureDataCustomer(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(60) };
            _restSharpHelper = new RestSharpHelper(configuration);

            // Inisialisasi DbContext global sekali di konstruktor
            var optionsBuilder = new DbContextOptionsBuilder<ConsoleDbContext>();
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DEV"),
                opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds));

            _dbContext = new ConsoleDbContext(optionsBuilder.Options, _configuration);
        }

        public async Task FetchCustomersAsync()
        {
            const string urlBase = "https://api.businesscentral.dynamics.com/v2.0/11560066-52c2-4a64-87e5-60e371425dd5/TRAIN/ODataV4/Company('CRONUS%20International%20Ltd.')/Customers";
            const int pageSize = 1000;

            try
            {
                var customers = await _restSharpHelper.GetDataAsync<Customer>(urlBase, pageSize);
                if (customers != null && customers.Any())
                {
                    await UpsertCustomersViaBulkEFAsync(customers);
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

        /// <summary>
        /// Upsert (insert or update) massal data Customer dengan BulkExtensions
        /// </summary>
        private async Task UpsertCustomersViaBulkEFAsync(List<Customer> customers)
        {
            var stopwatch = Stopwatch.StartNew();

            if (!customers.Any())
            {
                Console.WriteLine("No valid customer data.");
                return;
            }

            // 2️⃣ Ambil daftar pelanggan yang sudah ada di database untuk dibandingkan
            var existingCustomers = await _dbContext.Customers
                .AsNoTracking()
                .Select(c => c.No)
                .ToListAsync();

            var existingCustomerSet = new HashSet<string>(existingCustomers);

            // 3️⃣ Pisahkan antara data yang perlu di-insert dan di-update
            var customersToInsert = new List<Customer>();
            var customersToUpdate = new List<Customer>();

            foreach (var customer in customers)
            {
                if (existingCustomerSet.Contains(customer.No))
                {
                    customersToUpdate.Add(customer);
                }
                else
                {
                    customersToInsert.Add(customer);
                }
            }

            // 4️⃣ Upsert ke database dengan BulkExtensions
            try
            {
                var bulkConfig = new BulkConfig
                {
                    SetOutputIdentity = false,
                    PreserveInsertOrder = false,
                    BatchSize = 10000,
                    UseTempDB = false,
                };

                if (customersToInsert.Any())
                {
                    Console.WriteLine($"Inserting {customersToInsert.Count} new customers...");
                    await _dbContext.BulkInsertAsync(customersToInsert, bulkConfig);
                }

                if (customersToUpdate.Any())
                {
                    Console.WriteLine($"Updating {customersToUpdate.Count} existing customers...");
                    await _dbContext.BulkUpdateAsync(customersToUpdate, bulkConfig);
                }

                stopwatch.Stop();
                Console.WriteLine(
                    $"{customersToInsert.Count} inserted, {customersToUpdate.Count} updated. Completed in {stopwatch.Elapsed.TotalSeconds:F2} secs");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during Bulk Insert/Update: {ex.Message}");
            }
        }
    }
}