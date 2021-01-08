using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL_EXAM;
using Domain_EXAM;

namespace WebApp_EXAM.Pages_Examples
{
    public class CreateModel : PageModel
    {
        private readonly DAL_EXAM.AppDbContextExam _context;

        public CreateModel(DAL_EXAM.AppDbContextExam context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty] public Example Example { get; set; } = default!;

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Examples.Add(Example);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
