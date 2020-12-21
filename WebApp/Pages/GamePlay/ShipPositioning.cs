#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Table = Domain.Table;

namespace WebApp.Pages.GamePlay
{
    public class ShipPositioning : PageModel
    {
        public Game Game { get; set; } = null!;
        public Battleship Battleship { get; set; } = null!;
        
        private readonly ILogger<IndexModel> _logger;
        private readonly DAL.AppDbContext _context;
        private readonly WebAppDbContent _webAppDbContent;

        public ShipPositioning(DAL.AppDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
            _webAppDbContent = new WebAppDbContent(context);
        }

        public async Task<IActionResult> OnGetAsync(int id, string? dir, bool? placeShip )
        {
            Game = await _webAppDbContent.LoadGameById(id);
            Battleship = new Battleship(Game);

            if (dir != null)
            {
                if (Battleship.MoveShip(dir) == null)
                {
                    ModelState.AddModelError("outsideTheBorder", "The ship cannot be placed outside the border!");
                }
            }
            else if (placeShip != null)
            {
                if (Battleship.PlaceShip())
                {
                    if (Battleship.AreShipsPlaced())
                    {
                        return RedirectToPage("/GamePlay/Index", new {id = id});
                    }
                    Battleship.LoadGameStates();
                    Battleship.GenerateRandomShip(0);
                }
                else
                {
                    ModelState.AddModelError("shipPut", "Ship location is not valid!");
                }
            }
            else
            {
                Battleship.GenerateRandomShip(0);
                Battleship.LoadGameStates();
            }

            await _webAppDbContent.SaveGame(Battleship.GetGameData());

            return Page();
        }

    }
}