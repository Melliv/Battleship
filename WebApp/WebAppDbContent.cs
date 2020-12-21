using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace WebApp
{
    public class WebAppDbContent
    {
        private readonly AppDbContext _dbContext;
        
        public WebAppDbContent(AppDbContext context)
        {
            _dbContext = context;
        }
        
        public async Task<Game> LoadGameById(int gameId)
        {
            Game game = await _dbContext.Games
                .Where(d => d.GameId == gameId)
                .FirstOrDefaultAsync();

            List<Table> tables = await _dbContext.Tables
                .Where(d => d.GameId == gameId)
                .Include(d => d.ShipsToPlace)
                .ToListAsync();

            List<GameState> gameStates = await _dbContext.GameState
                .Where(d => d.GameId == gameId)
                .Include(d => d.ActiveCells)
                .OrderBy(g => g.GameStateIndex)
                .ToListAsync();
            
            foreach (var table in tables) // for some reason default ships are saved in the database in wrong order. This for loop sort them
            {
                if (table.ShipsToPlace == null) break;
                List<DefaultShip> defaultShips = table.ShipsToPlace.OrderByDescending(d => d.ShipLength).ToList();
                table.ShipsToPlace = defaultShips;
            }
            
            game.Tables = tables;
            game.GameStates = gameStates;

            return game;
        }
        
        public async Task<List<Game>> LoadAllGames()
        {
            List<Game> games = await _dbContext.Games.ToListAsync();
            return games;
        }
        
        public async Task SaveGame(Game game)
        {
            _dbContext.Games.Update(game);
            await _dbContext.SaveChangesAsync();
        }
    }
}