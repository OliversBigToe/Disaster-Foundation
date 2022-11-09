using Microsoft.EntityFrameworkCore;

namespace APPR6312_Assignment.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
                : base(options)
        {
            Database.EnsureCreated();
        }
            public DbSet<User> Users { get; set; }

            public DbSet<Cash> Money { get; set; }

            public DbSet<Disasters> Disaster { get; set; }

            public DbSet<Good> Goods { get; set; }

            public DbSet<Transaction> Transactions { get; set; }

            public DbSet<Inventories> Inventory { get; set; }

    }
}
