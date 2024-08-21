using ESGAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESGAPI.Contexts
{
    public class CustomerDbContext : DbContext
    {
        public DbSet<Customer> Customer { get; set; }

        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options) {
        }

        public CustomerDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
     }
}
