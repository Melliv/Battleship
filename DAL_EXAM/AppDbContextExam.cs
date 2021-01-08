using Domain_EXAM;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL_EXAM
{
    public class AppDbContextExam : IdentityDbContext
    {
        public DbSet<Example> Examples { get; set; } = default!;
        //public DbSet<Table> Tables { get; set; } = default!;
        //public DbSet<Cell> Cells { get; set; } = default!;
        //public DbSet<DefaultShip> DefaultShips { get; set; } = default!;
        //public DbSet<GameState> GameState { get; set; } = default!;

        public AppDbContextExam(DbContextOptions<AppDbContextExam> options)
            : base(options)
        {
        }
    }
}