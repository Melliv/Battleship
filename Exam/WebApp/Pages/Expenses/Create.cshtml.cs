using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages_Expenses
{
    public class CreateModel : PageModel
    {

        private readonly DAL.AppDbContext _context;

        public CreateModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["CarId"] = new SelectList(_context.Car, "CarId", "NumberPlate");
        ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name");
            return Page();
        }

        [BindProperty]
        public Expense? Expense { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Expense.Add(Expense!);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
