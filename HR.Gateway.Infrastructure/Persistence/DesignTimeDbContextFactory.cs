using System.IO;
using HR.Gateway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HR.Gateway.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<HRDbContext>
{
    public HRDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = config.GetConnectionString("Postgres")
                 ?? "Host=localhost;Port=5432;Database=hr_gateway_db;Username=hr_admin;Password=hr_password_DevOnly";

        var options = new DbContextOptionsBuilder<HRDbContext>()
            .UseNpgsql(cs)
            .Options;

        return new HRDbContext(options);
    }
}