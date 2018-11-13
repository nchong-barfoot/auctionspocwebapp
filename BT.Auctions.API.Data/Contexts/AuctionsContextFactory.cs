using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BT.Auctions.API.Data.Contexts
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory{BT.Auctions.API.Data.Contexts.AuctionsContext}" />
    /// <seealso cref="AuctionsContext" />
    public class AuctionsContextFactory : IDesignTimeDbContextFactory<AuctionsContext>
    {
        /// <summary>
        /// Creates a new instance of the auctions database context.
        /// </summary>
        /// <param name="args">Arguments provided by the design-time service.</param>
        /// <returns>
        /// An instance of <typeparamref name="TContext" />.
        /// </returns>
        public AuctionsContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            DbContextOptionsBuilder<AuctionsContext> builder = new DbContextOptionsBuilder<AuctionsContext>();

            string connectionString = configuration.GetConnectionString("AuctionsDbConnection");

            builder.UseSqlServer(connectionString);

            return new AuctionsContext(builder.Options);
        }
    }
}
