using BT.Auctions.API.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BT.Auctions.API.Data
{
    public class Program : IDesignTimeDbContextFactory<AuctionsContext>
    {
        static void Main(string[] args)
        {
            Program program = new Program();

            using (AuctionsContext auctionsContext = program.CreateDbContext(null))
            {
                auctionsContext.Database.Migrate();
                auctionsContext.SaveChanges();
            }
        }

        public AuctionsContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Constants.AppSettingsJson)
                .Build();

            DbContextOptionsBuilder<AuctionsContext> builder = new DbContextOptionsBuilder<AuctionsContext>();
            builder.UseLazyLoadingProxies();
            string connectionString = configuration.GetConnectionString(Constants.AuctionDbConnection);

            builder.UseSqlServer(connectionString);

            return new AuctionsContext(builder.Options);
        }
    }
}

