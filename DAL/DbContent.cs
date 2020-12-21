using System.Collections.Generic;
using System.Linq;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class DbContent
    {

        private readonly DbContextOptions<AppDbContext> _dbOption;
        
        public DbContent(DbContextOptions<AppDbContext> options)
        {
            _dbOption = options;
        }
        
        public Game LoadGameById(int gameId)
        {
            using var dbConnection = new AppDbContext(_dbOption);
            Game game = dbConnection.Games.FirstOrDefault(g => g.GameId == gameId)!;

            List<Table> tables = dbConnection.Tables
                .Where(t => t.GameId == gameId)
                .Include(d => d.ShipsToPlace)
                .ToList();
            
            List<GameState> gameStates = dbConnection.GameState
                .Where(g => g.GameId == gameId)
                .Include(c => c.ActiveCells)
                .OrderBy(g => g.GameStateIndex)
                .ToList();

            foreach (var table in tables) // for some reason default ships are saved in the database in wrong order. This for loop sort them
            {
                if (table.ShipsToPlace == null) break;
                List<DefaultShip> defaultShips = table.ShipsToPlace.OrderByDescending(d => d.ShipLength).ToList();
                table.ShipsToPlace = defaultShips;
            }
            
            game!.Tables = tables;
            game.GameStates = gameStates;

            return game;
        }
        
        public List<Game> LoadAllGames()
        {
            using var dbConnection = new AppDbContext(_dbOption);
            List<Game> games = dbConnection.Games.ToList();
            return games;
        }
        
        public void SaveGame(Game game)
        {
            using var dbConnection = new AppDbContext(_dbOption);
            dbConnection.Games.Update(game);
            dbConnection.SaveChanges();
        }
    }
}