using ConsoleBusinessCentral.Persistence;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ConsoleBusinessCentral.Persitence
{
    public class ConsoleDbContextFactory : IDesignTimeDbContextFactory<ConsoleDbContext>
    {
        public ConsoleDbContext CreateDbContext(string[] args)
        {
            var assemblyLocation = Path.GetDirectoryName(typeof(ConsoleDbContextFactory).Assembly.Location);

            var config = new ConfigurationBuilder()
                .SetBasePath(assemblyLocation)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var builder = new DbContextOptionsBuilder<ConsoleDbContext>();
            builder.UseSqlServer(config.GetConnectionString("DEV"));
            return new ConsoleDbContext(builder.Options, config);
        }
    }
}
