using EmailListFilter.Entity;
using EmailListFilter.Helper;
using Microsoft.EntityFrameworkCore;

namespace EmailListFilter.Context
{
    public class UserContext : DbContext
    {
        public UserContext()
        {
        }

        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL(Constants.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserIndexed>()
                .HasIndex(x => x.Email);
        }

        public DbSet<UserIndexed> UsersIndexed { get; set; }
        public DbSet<UserUnindexed> UsersUnindexed { get; set; }
    }
}
