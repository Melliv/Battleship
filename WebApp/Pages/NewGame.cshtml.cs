using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    public class NewGame : PageModel
    {
        private readonly DAL.AppDbContext _context;
        
        public NewGame(DAL.AppDbContext context)
        {
            _context = context;
        }
        
        [MaxLength(20)]
        [BindProperty(SupportsGet = true)]
        public string GameName { get; set; } = null!;

        [BindProperty(SupportsGet = false)]
        public string Player1Name { get; set; } = null!;

        [BindProperty(SupportsGet = false)]
        public string Player2Name { get; set; } = null!;
        [Range(3,20)]
        [BindProperty(SupportsGet = true)]
        public string X { get; set; } = null!;
        
        [Range(3,20)]
        [BindProperty(SupportsGet = true)]
        public string Y { get; set; } = null!;

        [BindProperty(SupportsGet = true)]
        public bool AutomatedShipsPos { get; set; }
        
        
        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine(ModelState.ErrorCount);
            if (GameName == null)
            {
                ModelState.AddModelError("null", "Game name can't be null!");
                return Page();
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Game game = new Game()
            {
                GameName = GameName,
                Width = Int32.Parse(X),
                Length = Int32.Parse(Y),
                AutomatedShipsPos = AutomatedShipsPos
            };
            
            Battleship battleship = new Battleship(game, Player1Name, Player2Name);

            _context.Games.Add(battleship.GetGameData());
            await _context.SaveChangesAsync();
            
            if (AutomatedShipsPos)
            {
                return RedirectToPage("./GamePlay/Index", new {id = game.GameId});
            }
            return RedirectToPage("./GamePlay/ShipPositioning", new {id = game.GameId});
        }

    }
}