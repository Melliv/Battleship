using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using GameBrain;
using GameBrain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Table = Domain.Table;

namespace WebApp.Pages.GamePlay
{
    public class Index : PageModel
    {
        public Game Game { get; set; } = null!;
        public Battleship Battleship { get; set; } = null!;
        public string? Winner { get; set; }
        public string? PlayerTurnName { get; set; }
        public string? TableState { get; set; }
        
        private readonly ILogger<IndexModel> _logger;
        private readonly DAL.AppDbContext _context;
        private readonly WebAppDbContent _webAppDbContent;

        public Index(DAL.AppDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
            _webAppDbContent = new WebAppDbContent(context);
        }

        public async Task<IActionResult> OnGetAsync(int id, int? tableIndex, int? x, int? y, bool? stepBack)
        {
            Game = await _webAppDbContent.LoadGameById(id);
            Battleship = new Battleship(Game);

            if (tableIndex != null && x != null && y != null )
            {
                var shootTableIndex = Battleship.GetInvertedTableIndex();

                if (tableIndex != shootTableIndex)
                {
                    ModelState.AddModelError("wrongTable", "You can't shoot this table!");
                }
                if (tableIndex == shootTableIndex || tableIndex == 1)
                {
                    TableState = Battleship.Shoot(shootTableIndex, x.Value, y.Value).ToString();
                }
                else
                {
                    Battleship.LoadGameStates();
                }
            }
            else if (stepBack != null)
            {
                if (Battleship.CanTakeStepBack())
                {
                    Battleship.TakeStepBack();
                }
                else
                {
                    ModelState.AddModelError("cantTakeStepBack", "Can't Take step back!");
                }
            }
            else
            {
                Battleship.LoadGameStates();
            }

            Winner = Battleship.GetWinnerName();
            PlayerTurnName = Battleship.GetNextTurnPlayerName();

            await _webAppDbContent.SaveGame(Battleship.GetGameData());

            return Page();
        }

    }
}