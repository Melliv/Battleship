using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages_Cars
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }
        [BindProperty(SupportsGet = true)] public string NumberPlate { get; set; } = "";
        [BindProperty(SupportsGet = true)] public string Mark { get; set; } = "";
        [BindProperty(SupportsGet = true)] public string Model { get; set; } = "";
        [BindProperty(SupportsGet = true)] public int ReleaseYear { get; set; }
        [BindProperty(SupportsGet = true)] public string Gearbox { get; set; } = "";
        [BindProperty(SupportsGet = true)] public string FuelType { get; set; } = "";
        [BindProperty(SupportsGet = true)] public int CarTypeId { get; set; }
        [BindProperty(SupportsGet = true)] public string? Btn { get; set; }
        public IList<Car>? Car { get;set; }
        public List<SelectListItem> CarTypes { get;set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync()
        {
            var cars = _context.Car
                .Include(c => c.CarType).AsQueryable();
            
            switch (Btn)
            {
                case "Search":
                    cars = !string.IsNullOrWhiteSpace(NumberPlate) 
                        ? cars!.Where(c => c.NumberPlate!.ToLower() == NumberPlate.ToLower().Trim()) 
                        : cars;
                    cars = !string.IsNullOrWhiteSpace(Mark) 
                        ? cars!.Where(c => c.Mark!.ToLower() == Mark.ToLower())
                        : cars;
                    cars = !string.IsNullOrWhiteSpace(Model) 
                        ? cars!.Where(c => c.Model!.ToLower() == Model.ToLower().Trim())
                        : cars;
                    cars = ReleaseYear != 0
                        ? cars!.Where(c => c.ReleaseYear >= ReleaseYear)
                        : cars;
                    cars = !string.IsNullOrWhiteSpace(Gearbox) 
                        ? cars!.Where(c => c.Gearbox!.ToLower() == Gearbox.ToLower().Trim())
                        : cars;
                    cars = !string.IsNullOrWhiteSpace(FuelType) 
                        ? cars!.Where(c => c.FuelType!.ToLower() == FuelType.ToLower().Trim())
                        : cars;
                    cars = CarTypeId != 0
                        ? cars!.Where(c => c.CarTypeId == CarTypeId)
                        : cars;
                    Btn = "Reset";
                    break;
                case "Reset":
                    Btn = "";
                    NumberPlate = "";
                    Mark = ""; 
                    Model = "";
                    ReleaseYear = 0;
                    Gearbox = "";
                    FuelType = "";
                    CarTypeId = 0;
                    return RedirectToPage("/Cars/Index");
            }

            await LoadAllCarTypes();
            Car = await cars.ToListAsync();
            await AddExpenses();
            return Page();
        }

        private async Task LoadAllCarTypes()
        {
            CarTypes.Add(new SelectListItem("", ""));
            List<CarType> carTypes = await _context.CarType.ToListAsync();
            foreach (var carType in carTypes)
            {
                CarTypes.Add(new SelectListItem(carType.Name,carType.CarTypeId.ToString()));
            }
            
        }
        
        private async Task AddExpenses()
        {
            List<Expense> expenses = await _context.Expense.ToListAsync();
            foreach (var car in Car!)
            {
                foreach (var expense in expenses.Where(expense => car.CarId == expense.CarId))
                {
                    car.Expenses ??= new List<Expense>();
                    car.Expenses.Add(expense);
                }
            }
        }
        
    }
}
