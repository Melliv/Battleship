using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL_EXAM;
using Domain_EXAM;

namespace WebApp_EXAM.Pages_Examples
{
    public class EditModel : PageModel
    {
        private readonly DAL_EXAM.AppDbContextExam _context;

        public EditModel(DAL_EXAM.AppDbContextExam context)
        {
            _context = context;
        }

        [BindProperty]
        public Example Example { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Example = await _context.Examples.FirstOrDefaultAsync(m => m.ExampleId == id);

            if (Example == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Example).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExampleExists(Example.ExampleId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ExampleExists(int? id)
        {
            return _context.Examples.Any(e => e.ExampleId == id);
        }
    }
}
