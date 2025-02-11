using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using ConsoleBusinessCentral.Extensions;

var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = builder.Build();

var configureData = new ConfigureDataCustomer(configuration);

try
{
    Console.WriteLine("Get Data...");
    var stopwatch = Stopwatch.StartNew();

    await configureData.FetchCustomersAsync();

    stopwatch.Stop();
    Console.WriteLine($"Data fetch and processing completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds");
    Console.ReadKey();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
