using EmployeeProducer.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProducer.API.Data
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }
        public DbSet<Employee> Employees { get; set; }
    }
}
