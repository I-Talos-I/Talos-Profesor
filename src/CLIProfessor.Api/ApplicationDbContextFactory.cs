using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using CLIProfessor.Infrastructure.Persistence;

namespace CLIProfessor.Api;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Build configuration
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            // Fallback for local development if not set, BUT without sensitive data if possible
            // Or throw exception to force user to set it.
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found. Set it in appsettings.json or environment variables.");
        }

        optionsBuilder.UseNpgsql(connectionString, o => o.UseVector());

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
