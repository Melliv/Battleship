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
    public class IndexModel : PageModel
    {
        private readonly DAL_EXAM.AppDbContextExam _context;

        public IndexModel(DAL_EXAM.AppDbContextExam context)
        {
            _context = context;
        }

        public IList<Example> Example { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Example = await _context.Examples.ToListAsync();
        }
    }
}
