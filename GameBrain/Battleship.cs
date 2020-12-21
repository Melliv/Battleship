using System.Collections.Generic;
using System.Linq;
using Domain;
using GameBrain.Enums;

namespace GameBrain
{
    public class Battleship
    {
        private readonly Game _game;
        
        private readonly List<Table> _tables = new List<Table>();

        public Battleship(Game game)
        {
            _game = game;
            if (_game.Tables != null)
            {
                foreach (var tableData in _game.Tables)
                {
                    var newTable = new Table(tableData);
                    _tables.Add(newTable);
                }
            }

            _game.GameStates ??= new List<GameState>();
            
        }
        
        public Battleship(Game game, string player1Name, string player2Name)
        {
            _game = game;
            var (shipsToPlace, cellsCount) = CalculateShipsCount();
            var tableData1 = new Domain.Table()
            {
                PlayerName = player1Name,
                Width = _game.Width,
                Length = _game.Length,
                CellsToShoot = cellsCount,
                BlindTable = false,
                ShipsToPlace = shipsToPlace,
            };
            var tableData2 = new Domain.Table()
            {
                PlayerName = player2Name,
                Width = _game.Width,
                Length = _game.Length,
                CellsToShoot = cellsCount,
                BlindTable = true,
                ShipsToPlace = shipsToPlace.Select(item => item.Clone()).ToList()
            };
            var newTable1 = new Table(tableData1);
            var newTable2 = new Table(tableData2);
            _tables.Add(newTable1);
            _tables.Add(newTable2);
            _game.Tables = new List<Domain.Table> {tableData1, tableData2};
            _game.GameStates = new List<GameState>();

            if (_game.AutomatedShipsPos)
            {
                PlaceAllShips(0); // Place player ships
            }
            PlaceAllShips(1); // Place computer ships
        }

        private void PlaceAllShips(int tableIndex)
        {
            do { } while (GenerateRandomShip(tableIndex));
        }
        public bool GenerateRandomShip(int tableIndex)
        {
            if (tableIndex == -1) // if app don't know which table need ship
            {
                tableIndex = GetInvertedTableIndex();
            }

            if (!_tables[tableIndex].CanGenerateRandomShip()) return false;

            var gameState = _tables[tableIndex].GenerateRandomShip();

            gameState.GameStateIndex = _game.GameStates!.Count;
            gameState.TableIndex = tableIndex;
            _game.GameStates!.Add(gameState);
            return true;
        }

        public void LoadGameStates()
        {
            if (_game.GameStates == null) return;
            foreach (var gameState in _game.GameStates)
            {
                LoadGameState(gameState);
            }
        }
        
        private void LoadGameState(GameState gameState)
        {
            _tables[gameState.TableIndex].LoadGameState(gameState);
        }

        public GameState? MoveShip(string dir)
        {
            if (_game.GameStates!.Count == 0) return null;
            
            var oldGameState = _game.GameStates.Last();
            var tableIndex = oldGameState.TableIndex;
            var digDir = dir switch
            {
                "rotate" => new[] {0, 0},
                "up" => new[] {0, -1},
                "left" => new[] {-1, 0},
                "down" => new[] {0, 1},
                "right" => new[] {1, 0},
                _ => new int[2]
            };

            if (!_tables[tableIndex].CanMoveShip(oldGameState, digDir))
            {
                LoadGameStates();
                return null;
            }

            List<Cell> activeCells= _tables[tableIndex].MoveShip(oldGameState, digDir);

            GameState newGameState = new GameState
            {
                TableIndex = tableIndex,
                ActiveCells = activeCells,
                GameStateIndex = _game.GameStates.Count
            };
            
            _game.GameStates.Remove(oldGameState);
            _game.GameStates.Add(newGameState);
            LoadGameStates();
            return oldGameState;

        }

        public bool PlaceShip()
        {
            ClearGameBoards();
            GameState lastGameState = _game.GameStates!.Last();
            _game.GameStates!.Remove(lastGameState);
            LoadGameStates();
            var canPlaceShip = _tables[lastGameState.TableIndex].PlaceShip(lastGameState);
            _game.GameStates.Add(lastGameState);
            LoadGameState(lastGameState);
            return canPlaceShip;
        }

        public EShotResult Shoot(int shootTableIndex, int x, int y)
        {
            if (!_game.PlayerOneTurn)
            {
                LoadGameStates();
                (x, y) = _tables[0].GetShootCord();
            }

            (Cell? shotCell, EShotResult result) = LoadAllStatesShot(shootTableIndex, x, y);
            
            if (result == EShotResult.Passed)
            {
                _tables[shootTableIndex].PlaceBomb(x, y);
                shotCell = new Cell()
                {
                    X = x,
                    Y = y,
                };
            }

            if (result != EShotResult.DoubleHit)
            {
                GameState newGameState = new GameState
                {
                    ActiveCells = new List<Cell> {shotCell!}, 
                    TableIndex = shootTableIndex,
                    GameStateIndex = _game.GameStates!.Count
                };
                _game.GameStates!.Add(newGameState);
                _game.BombsCount++;
            }
            
            if (result == EShotResult.Passed || result == EShotResult.DoubleHit)
            {
                _game.PlayerOneTurn = shootTableIndex == 0;
            }
            
            return result;
        }

        private (Cell?, EShotResult) LoadAllStatesShot(int shootTableIndex, int x, int y)
        {
            Cell? shotCell = null;
            var result = EShotResult.Passed;
            foreach (var gameState in _game.GameStates!)
            {
                if (gameState.TableIndex == shootTableIndex)
                {
                    (Cell? gCell,var gResult) = _tables[shootTableIndex].LoadGameStateShot(gameState, x, y);
                    if (gResult != EShotResult.Passed)
                    {
                        result = gResult;
                    }
                    if (gCell != null)
                    {
                        gameState.ActiveCells!.Remove(gCell);
                        gCell.GameStateIndex = gameState.GameStateIndex; // to know where cell belonged
                        shotCell = gCell;
                    }
                }
                else
                {
                    LoadGameState(gameState);
                }
            }
            return (shotCell, result);
        }

        public bool CanTakeStepBack()
        {
            if (_game.BombsCount != 0) return true;
            LoadGameStates();
            return false;
        }
        public void TakeStepBack()
        {
            _game.BombsCount--;

            var lastGameState = _game.GameStates!.Last();
            _game.GameStates!.Remove(lastGameState);
            
            var shotCell = lastGameState.ActiveCells!.First();
            lastGameState.ActiveCells!.Remove(shotCell);

            switch (shotCell.BombShipBoth)
            {
                case 0:
                    _game.PlayerOneTurn = !_game.PlayerOneTurn;
                    break;
                case 2:
                {
                    var gameState = _game.GameStates
                        .First(g => g.GameStateIndex == shotCell.GameStateIndex!);

                    if (shotCell.GameStateId != 0)
                    {
                        shotCell.GameStateId = gameState.GameStateId;
                    }
                    
                    shotCell.GameState = gameState;
                    shotCell.BombShipBoth = 1;
                    _tables[gameState.TableIndex].AddShipsCellsCount();
                    gameState.ActiveCells!.Add(shotCell);
                    break;
                }
            }

            ClearGameBoards();
            LoadGameStates();
        }

        private (List<DefaultShip>, int) CalculateShipsCount()
        {
            var cellsCount = 0;
            int[] standardShips = {5, 4, 3, 3, 2, 2, 1, 1};
            var shipsToPlace = new List<DefaultShip>();
            var capacity = (int) (_game.Width * _game.Length * 0.2);
            while (capacity != 0)
            {
                foreach (var shipLength in standardShips)
                {
                    if (capacity >= shipLength)
                    {
                        var defaultShip = new DefaultShip {ShipLength = shipLength};
                        shipsToPlace.Add(defaultShip);
                        cellsCount += shipLength;
                        capacity -= shipLength;
                    } else if (capacity == 0)
                    {
                        break;
                    }
                }
            }
            shipsToPlace.Sort((x, y) => x.ShipLength.CompareTo(y.ShipLength));
            shipsToPlace.Reverse();
            return (shipsToPlace, cellsCount);
        }
        
        public Game GetGameData()
        {
            return _game;
        }

        public Table GetTable(int tableIndex)
        {
            return _tables[tableIndex];
        }
        
        public void ClearGameBoards()
        {
            _tables[0].RemoveBoard();
            _tables[1].RemoveBoard();
        }

        public int GetInvertedTableIndex()
        {
            return _game.PlayerOneTurn ? 1 : 0;
        }

        private int GetTableIndex()
        {
            return _game.PlayerOneTurn ? 0 : 1;
        }
        
        public string GetNextTurnPlayerName()
        {
            return _tables[GetTableIndex()].GetTableData().PlayerName!;
        }

        public bool AreShipsPlaced()
        {
            return _tables[0].GetTableData().ShipsToPlace!.Count == 0
                   && _tables[1].GetTableData().ShipsToPlace!.Count == 0;
        }
        
        public string? GetWinnerName()
        {
            if (_tables[0].GetTableData().CellsToShoot == 0)
            {
                return _tables[1].GetTableData().PlayerName!;
            }
            if (_tables[1].GetTableData().CellsToShoot == 0)
            {
                return _tables[0].GetTableData().PlayerName!;
            }
            return null;
        }
    }
}