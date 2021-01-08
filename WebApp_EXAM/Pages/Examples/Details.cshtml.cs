using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL_EXAM;
using Domain_EXAM;

namespace WebApp_EXAM.Pages_Examples
{
    public class DetailsModel : PageModel
    {
        private readonly DAL_EXAM.AppDbContextExam _context;

        public DetailsModel(DAL_EXAM.AppDbContextExam context)
        {
            _context = context;
        }

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
    }
}
