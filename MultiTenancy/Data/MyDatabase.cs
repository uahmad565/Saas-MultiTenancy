using Microsoft.EntityFrameworkCore;
using MultiTenancy.Services;

namespace MultiTenancy.Data
{
    public class MyDatabase : DbContext
    {
        private readonly string tenant;
        public DbSet<Animal> Animals { get; set; } = default!;
        public MyDatabase(DbContextOptions<MyDatabase> options, ITenantGetter tenantGetter)
            : base(options)
        {
            tenant = tenantGetter.Tenant;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Animal>()
                .HasQueryFilter(a => a.Tenant == tenant)
                .HasData(
                    new() { Id = 1, Kind = "Dog", Name = "Samson", Tenant = "Khalid" },
                    new() { Id = 2, Kind = "Dog", Name = "Guinness", Tenant = "Khalid" },
                    new() { Id = 3, Kind = "Cat", Name = "Grumpy Cat", Tenant = "Internet" },
                    new() { Id = 4, Kind = "Cat", Name = "Mr. Bigglesworth", Tenant = "Internet" }
                );
        }
    }

    public class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Kind { get; set; } = string.Empty;
        public string Tenant { get; set; } = Tenants.Internet;
    }
}
