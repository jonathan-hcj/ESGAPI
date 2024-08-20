using ESGAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESGAPI.Contexts
{
    public class CustomerDbContext : DbContext
    {
        public DbSet<Customer> Customer { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=JONATHANS_LT\\SQLEXPRESS;Initial Catalog=ESG;Persist Security Info=True;User ID=API1;Password=Blat;Trust Server Certificate=True;");

        }

     }
}
