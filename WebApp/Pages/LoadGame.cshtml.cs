﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApp.Pages
{
    public class LoadGame : PageModel
    {

        [BindProperty(SupportsGet = true)] 
        public List<Game> Games { get; set; } = null!;
        
        private readonly ILogger<IndexModel> _logger;
        private readonly DAL.AppDbContext _context;
        private readonly WebAppDbContent _webAppDbContent;

        public LoadGame(DAL.AppDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
            _webAppDbContent = new WebAppDbContent(context);
        }
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id != null)
            {
                return RedirectToPage("/GamePlay/Index", new {id = id});
            }
            Games = await _webAppDbContent.LoadAllGames();
            return Page();
        }

    }
}