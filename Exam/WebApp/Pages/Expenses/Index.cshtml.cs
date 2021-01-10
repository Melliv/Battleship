using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages_Expenses
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)] public string? Description { get; set; }
        [BindProperty(SupportsGet = true)] public int CategoryId { get; set; }
        [BindProperty(SupportsGet = true)] public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        [BindProperty(SupportsGet = true)] public string? Btn { get; set; }
        public IList<Expense>? Expense { get;set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var expenses = _context.Expense
                .Include(e => e.Car)
                .Include(e => e.Category).AsQueryable();
            
            switch (Btn)
            {
                case "Search":
                    expenses = !string.IsNullOrWhiteSpace(Description) 
                        ? expenses!.Where(c => c.Description!.ToLower().Contains(Description.ToLower())) 
                        : expenses;
                    expenses = CategoryId != 0
                        ? expenses!.Where(c => c.CategoryId == CategoryId)
                        : expenses;
                    Btn = "Reset";
                    break;
                case "Reset":
                    Btn = "";
                    Description = "";
                    CategoryId = 0;
                    return RedirectToPage("/Expenses/Index");
            }

            await LoadAllCategories();
            Expense = await expenses.ToListAsync();
            return Page();
        }

        private async Task LoadAllCategories()
        {
            Categories.Add(new SelectListItem("", ""));
            List<Category> carTypes = await _context.Category.ToListAsync();
            foreach (var carType in carTypes)
            {
                Categories.Add(new SelectListItem(carType.Name,carType.CategoryId.ToString()));
            }
        }
    }
}
