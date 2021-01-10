using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<Car> Car { get; set; } = default!;
        public DbSet<Expense> Expense { get; set; } = default!;
        public DbSet<Category> Category { get; set; } = default!;
        public DbSet<CarType> CarType { get; set; } = default!;

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}