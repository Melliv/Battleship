using System.Collections.Generic;
using ConsoleApp.Exceptions;
using DAL;
using Domain;
using GameBrain;

namespace ConsoleApp
{

    public class MenuController
    {

        private readonly DbContent _dbContent;
        private readonly ConsoleUi _consoleUi = new ConsoleUi();
        private Battleship _battleship = null!;

        public MenuController(DbContent dbContent)
        {
            _dbContent = dbContent;
        }

        public void MakeStartMenu()
        {
            var menu = new Menu(MenuLevel.Level0);
            menu.AddMenuItem(new MenuItem("New game", "1", MakeNewGame));
            menu.AddMenuItem(new MenuItem("Load Game", "2", LoadAllGame));
            menu.RunMenu();
        }

        private void LoadAllGame()
        {
            List<Game> games = _dbContent.LoadAllGames();
            if (games.Count == 0)
            {
                _consoleUi.DisplayError("There are no saves to load!");
                throw new ReturnToMenuException();
            }

            var menu = new Menu(MenuLevel.Level1);
            var counter = 1;
            foreach (Game game in games)
            {
                string gameValue = game.GameName + " : " + game.Date + " : " + game.Width + " x " + game.Length;
                menu.AddMenuItem(new MenuItem(gameValue, counter.ToString(), () =>
                {
                    var newGame = _dbContent.LoadGameById(game.GameId);
                    _battleship = new Battleship(newGame);
                    _battleship.LoadGameStates();
                    MakeSootMenu();
                }));
                counter++;
            }
            menu.RunMenu();
        }
        private void MakeNewGame()
        {
            string gameName = _consoleUi.AskGameName();
            (string playerName1, string playerName2) = _consoleUi.AskPlayersName(1);
            (int x, int y) = _consoleUi.AskTableSize();
            bool automatedShipPos = _consoleUi.AskShipPosSet();
            
            // for testing
            //string gameName = "test";
            //string playerName1 = "peeter";
            //string playerName2 = "computer";
            //int x = 4;
            //int y = 4;
            //bool automatedShipPos = true;
            
            var game = new Game()
            {
                GameName = gameName,
                Width = x,
                Length = y,
                AutomatedShipsPos = automatedShipPos
            };
            
            _battleship = new Battleship(game, playerName1, playerName2);

            if (automatedShipPos)
            {
                MakeSootMenu();
            }
            else
            {
                MakePlaceShipMenu();
            }
        }
        
        private void MakeSootMenu()
        {
            _consoleUi.DisplayBoards(_battleship);
            
            var menu = new Menu(MenuLevel.Level1); 
            menu.AddMenuItem(new MenuItem("Make a move", "1", MakeAMove));
            menu.AddMenuItem(new MenuItem("Take step back", "2", TakeStepBack));
            menu.AddMenuItem(new MenuItem("Save Game", "s", SaveGame));
            menu.RunMenu();
        }
        
        private void MakePlaceShipMenu()
        {
            _battleship.GenerateRandomShip(0);
            _consoleUi.DisplayBoards(_battleship);
            
            var menu = new Menu(MenuLevel.Level1);
            menu.AddMenuItem(new MenuItem("Suitable", "1", PlaceShip));
            menu.AddMenuItem(new MenuItem("Move the ship", "2", MakeMoveShipMenu));
            menu.RunMenu();
        }
        
        private void MakeMoveShipMenu()
        {
            _consoleUi.DisplayBoards(_battleship);
            
            var menu = new Menu(MenuLevel.Level1);
            menu.AddMenuItem(new MenuItem("Suitable", "1", PlaceShip));
            menu.AddMenuItem(new MenuItem("^", "w", () =>
            {
                MoveTheShip("up");
            }));
            menu.AddMenuItem(new MenuItem(">", "d", () =>
            {
                MoveTheShip("right");
            }));
            menu.AddMenuItem(new MenuItem("<", "a", () =>
            {
                MoveTheShip("left");
            }));
            menu.AddMenuItem(new MenuItem( "v", "s", () =>
            {
                MoveTheShip("down");
            }));
            menu.AddMenuItem(new MenuItem( "rotate", "r", () =>
            {
                MoveTheShip("rotate");
            }));
            menu.RunMenu();
        }
        
        private void MakeAMove()
        {
            _battleship.ClearGameBoards(); // Boards needs to be empty before shoot cell
            _battleship.LoadGameStates(); // load all boards by looking at the gameStates
            
            int x = -1; // -1 will give an error in gameBrain
            int y = -1;
            int shootTableIndex = _battleship.GetInvertedTableIndex();
            int width = _battleship.GetGameData().Width;
            int length = _battleship.GetGameData().Length;
            string playerName = _battleship.GetTable(shootTableIndex == 1 ? 0 : 1).GetTableData().PlayerName!;

            if (shootTableIndex == 1)
            {
                (x, y) = _consoleUi.AskShootCell(playerName, width, length);
            }

            string tableState = _battleship.Shoot(shootTableIndex, x, y).ToString();
            var playerTurnName = _battleship.GetNextTurnPlayerName();
            var winner = _battleship.GetWinnerName();
            
            if (tableState != "")
            {
                _consoleUi.DisplayNotification("Player action: '" + tableState + "'");
            }
            
            if (winner == null)
            {
                _consoleUi.DisplayNotification("Player '" + playerTurnName + "' turn");
            }
            else
            {
                _consoleUi.DisplayNotification("Winner: " + winner);
            }

            _consoleUi.DisplayBoards(_battleship);
        }

        private void TakeStepBack()
        {
            if (!_battleship.CanTakeStepBack())
            {
                _consoleUi.DisplayError("Can not take a step back!");
            }
            else
            {
                _battleship.TakeStepBack();
            }
            _consoleUi.DisplayBoards(_battleship);
        }

        private void MoveTheShip(string direction)
        {
            _battleship.ClearGameBoards();
            
            if (_battleship.MoveShip(direction) == null)
            {
                _consoleUi.DisplayError("Ship position is unacceptable!");
            }

            _consoleUi.DisplayBoards(_battleship);
        }
        
        private void PlaceShip()
        {
            if (_battleship.PlaceShip())
            {
                if (_battleship.AreShipsPlaced())
                {
                     MakeSootMenu();
                }
                _battleship.GenerateRandomShip(0);
            }
            else
            {
                _consoleUi.DisplayError("Ship position is unacceptable!");
            }
            _consoleUi.DisplayBoards(_battleship);
        }

        private void SaveGame()
        {
            _dbContent.SaveGame(_battleship.GetGameData());
            _consoleUi.DisplayNotification("Game is saved!");
            throw new ReturnToMenuException();
        }
        

    }
}