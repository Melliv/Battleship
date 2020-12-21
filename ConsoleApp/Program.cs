using DAL;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp
{
    class Program
    {

        static void Main(string[] args)
        {
            DbContextOptions<AppDbContext> dbOption = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(
                @"Server=barrel.itcollege.ee,1533;
                                User Id=student;
                                Password=Student.Bad.password.0;
                                Database=villem.madisson_BattleshipI;
                                MultipleActiveResultSets=true;").Options;

            DbContent dbContent = new DbContent(dbOption);
            MenuController menuController = new MenuController(dbContent);
            menuController.MakeStartMenu();
            
        }
        
    }
}
