using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext : IdentityDbContext
    {
        public DbSet<Game> Games { get; set; } = default!;
        public DbSet<Table> Tables { get; set; } = default!;
        public DbSet<Cell> Cells { get; set; } = default!;
        public DbSet<DefaultShip> DefaultShips { get; set; } = default!;
        public DbSet<GameState> GameState { get; set; } = default!;

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}