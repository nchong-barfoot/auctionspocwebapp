using System;
using BT.Auctions.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BT.Auctions.API.Data.Contexts
{
    public class AuctionsContext : DbContext
    {
        public AuctionsContext(DbContextOptions<AuctionsContext> options) : base(options)
        {
        }

        public DbSet<Media> Medias { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<AuctionSession> AuctionSessions { get; set; }
        public DbSet<Lot> Lots { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<LotDetail> LotDetail { get; set; }
        public DbSet<DisplayConfiguration> DisplayConfigurations { get; set; }
        public DbSet<Display> Displays { get; set; }
        public DbSet<DisplayGroup> DisplayGroups { get; set; }
        public DbSet<DisplayGroupConfigurations> DisplayGroupConfigurations { get; set; }
        public DbSet<Bid> Bids { get; set; }

        /// <summary>
        /// Override this method to further configure the model that was discovered by convention from the entity types
        /// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
        /// and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
        /// define extension methods on this object that allow you to configure aspects of the model that are specific
        /// to a given database.</param>
        /// <remarks>
        /// If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
        /// then this method will not be run.
        /// Provides the ability to dictate table names if required.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Media>().ToTable("Media");
            modelBuilder.Entity<Venue>().ToTable("Venues");
            modelBuilder.Entity<Image>().ToTable("Images").Property(i => i.IsDisplayed).HasDefaultValue(true);
            
            modelBuilder.Entity<Lot>().ToTable("Lots").Property(l => l.AuctionStatus).HasConversion(v => v.ToString(),
                v => (AuctionStatus) Enum.Parse(typeof(AuctionStatus), v));
            modelBuilder.Entity<LotDetail>().ToTable("LotDetails").HasIndex(a => new {a.Key, a.LotId}).IsUnique();
            modelBuilder.Entity<Bid>().ToTable("Bids");

            modelBuilder.Entity<Display>().ToTable("Displays");
            modelBuilder.Entity<Display>().HasOne(k => k.Venue).WithMany().OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Display>().HasMany(k => k.DisplayConfigurations).WithOne().OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DisplayConfiguration>().ToTable("DisplayConfigurations").Property(l => l.DisplayMode).HasConversion(v => v.ToString(),
                v => (DisplayMode)Enum.Parse(typeof(DisplayMode), v));
            modelBuilder.Entity<DisplayConfiguration>().HasOne(k => k.Display).WithMany(d => d.DisplayConfigurations)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DisplayGroupConfigurations>().ToTable("DisplayGroupConfigurations")
                .HasKey(a => new { a.DisplayConfigurationId, a.DisplayGroupId });
            modelBuilder.Entity<DisplayGroupConfigurations>().HasOne(d => d.DisplayConfiguration)
                .WithMany(d => d.DisplayGroupConfigurations).HasForeignKey(l => l.DisplayConfigurationId);
            modelBuilder.Entity<DisplayGroupConfigurations>().HasOne(d => d.DisplayGroup)
                .WithMany(d => d.DisplayGroupConfigurations).HasForeignKey(l => l.DisplayGroupId);

            modelBuilder.Entity<AuctionSession>().ToTable("AuctionSessions").HasOne(a => a.DisplayGroup).WithMany(g => g.AuctionSessions);
            modelBuilder.Entity<AuctionSession>().HasIndex(a => new { a.StartDate, a.FinishDate });
            modelBuilder.Entity<DisplayGroup>().ToTable("DisplayGroups").HasMany(d => d.AuctionSessions)
                .WithOne(d => d.DisplayGroup).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<DisplayGroup>().HasOne(k => k.Venue).WithMany()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
