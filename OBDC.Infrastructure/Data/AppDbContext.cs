using Microsoft.EntityFrameworkCore;
using OBDC.Core.Models;

namespace OBDC.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<PlayerAccount> PlayerAccounts { get; set; }
        public DbSet<CasinoWager> CasinoWagers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerAccount>()
                .HasKey(p => p.AccountId);

            modelBuilder.Entity<CasinoWager>()
                .HasKey(c => c.WagerId);

            modelBuilder.Entity<CasinoWager>()
                .HasIndex(c => c.AccountId);
        }
    }
}
