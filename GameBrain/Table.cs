using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using GameBrain.Enums;

namespace GameBrain
{
    public class Table
    {
        private ECellState[,] Board { get; set; }
        private Domain.Table TableData { get; set; }
        
        private readonly int[][] _surroundings = {new [] {0, 0}, 
            new [] {-1, 0}, new [] {1, 0}, new [] {0, 1}, new [] {0, -1},
            new [] {-1, 1}, new [] {-1, -1}, new [] {1, 1}, new [] {1, -1}
        };
        public Table(Domain.Table tableData)
        {
            TableData = tableData;
            Board = new ECellState[tableData.Length, tableData.Width];
        }

        public bool CanGenerateRandomShip()
        {
            return TableData.ShipsToPlace!.Count > 0;
        }
        
        public GameState GenerateRandomShip()
        {
            DefaultShip shipToPlace = TableData.ShipsToPlace!.FirstOrDefault()!;
            Random random = new Random();
            int randomX;
            int randomY;
            int[] randomDir;
            do
            {
                randomX = random.Next(0, TableData.Width);
                randomY = random.Next(0, TableData.Length);
                randomDir = _surroundings[random.Next(1, 5)];
            } while (!ValidateShipPos(randomX, randomY, shipToPlace!.ShipLength, randomDir));
            TableData.ShipsToPlace!.Remove(shipToPlace);
            return PlaceRandomShip(randomX, randomY, shipToPlace!.ShipLength, randomDir);
        }

        private bool ValidateShipPos(int randomX, int randomY, int shipLength, int[] randomDir)
        {
            for (var i = 0; i < shipLength; i++)
            {
                var first = true;
                foreach (var surroundingCell in _surroundings)
                {
                    try
                    {
                        var x = randomX + surroundingCell[0];
                        var y = randomY + surroundingCell[1];
                        if (Board[y, x] == ECellState.Ship)
                        {
                            return false;
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        if (first)
                        {
                            return false;
                        }
                    }
                    first = false;
                }

                randomX += randomDir[0];
                randomY += randomDir[1];
            }
            return true;
        }

        private GameState PlaceRandomShip(int x, int y, int shipLength, int[] dir)
        {
            GameState gameState = new GameState {ActiveCells = new List<Cell>()};
            for (var i = 0; i < shipLength; i++)
            {
                Board[y, x] = ECellState.Ship;
                Cell cell = new Cell()
                {
                    X = x,
                    Y = y,
                    BombShipBoth = 1
                };
                gameState.ActiveCells.Add(cell);
                x += dir[0];
                y += dir[1];
            }
            return gameState;
        }
        
        public bool PlaceShip(GameState gameState)
        {
            return gameState.ActiveCells!.All(cell => ValidateShipPos(cell.X, cell.Y, 1, new[] {0, 0}));
        }

        public bool CanMoveShip(GameState oldGameState, int[] dir)
        {
            foreach (var cell in oldGameState.ActiveCells)
            {
                var x = cell.X + dir[0];
                var y = cell.Y + dir[1];
                if (dir[0] == 0 && dir[1] == 0) // when direction 'rotate' switch x and y 
                {
                    var tempData = x;
                    x = y;
                    y = tempData;
                }
                if (x < 0 || x >= TableData.Width || y < 0 || y >= TableData.Length)
                {
                    return false;
                }
            }

            return true;
        }

        public List<Cell> MoveShip(GameState oldGameState, int[] dir)
        {
            List<Cell> activeCells = new List<Cell>();
            foreach (var cell in oldGameState.ActiveCells!)
            {
                var x = cell.X + dir[0];
                var y = cell.Y + dir[1];
                
                if (dir[0] == 0 && dir[1] == 0)
                {
                    var tempData = x;
                    x = y;
                    y = tempData;
                }
                
                Cell newCell = new Cell()
                {
                    X = x,
                    Y = y,
                    BombShipBoth = 1
                };
                activeCells.Add(newCell);
                Board[cell.Y, cell.X] = ECellState.Empty;
            }
            return activeCells;
        }

        public void LoadGameState(GameState gameState)
        {
            foreach (var cell in gameState.ActiveCells!)
            {
                Board[cell.Y, cell.X] = cell.BombShipBoth switch
                {
                    0 => ECellState.Bomb,
                    1 => ECellState.Ship,
                    2 => ECellState.ShipAndBomb,
                    _ => Board[cell.Y, cell.X]
                };
            }
        }
        
        public (Cell?, EShotResult) LoadGameStateShot(GameState gameState, int x, int y)
        {
            Cell? shotCell = null;
            EShotResult shotResult = EShotResult.Passed;
            foreach (var cell in gameState.ActiveCells!)
            {
                switch (cell.BombShipBoth)
                {
                    case 0 when cell.X == x && cell.Y == y:
                        shotResult = EShotResult.DoubleHit;
                        Board[cell.Y, cell.X] = ECellState.Bomb;
                        break;
                    case 0:
                        Board[cell.Y, cell.X] = ECellState.Bomb;
                        break;
                    case 1 when cell.X == x && cell.Y == y:
                    {
                        TableData.CellsToShoot--;
                        cell.BombShipBoth = 2;
                        shotCell = cell;
                        shotResult = gameState.ActiveCells.Count == 1 ? EShotResult.Destroyed : EShotResult.Hit;
                        Board[cell.Y, cell.X] = ECellState.ShipAndBomb;
                        break;
                    }
                    case 1:
                        Board[cell.Y, cell.X] = ECellState.Ship;
                        break;
                    case 2 when cell.X == x && cell.Y == y:
                    {
                        shotResult = EShotResult.DoubleHit;
                        Board[cell.Y, cell.X] = ECellState.ShipAndBomb;
                        break;
                    }
                    case 2:
                    {
                        Board[cell.Y, cell.X] = ECellState.ShipAndBomb;
                        break;
                    }

                }
            }

            return (shotCell, shotResult);
        }

        public (int, int) GetShootCord()
        {
            var random = new Random();
            do
            {
                var x = random.Next(0, TableData.Width);
                var y = random.Next(0, TableData.Length);
                if (Board[y, x] == ECellState.Empty || Board[y, x] == ECellState.Ship) 
                { 
                    return (x, y); 
                }
            } while (true);
        }
        
        public void PlaceBomb(int x, int y)
        {
            Board[y, x] = ECellState.Bomb;
        }
        
        public ECellState GetCell(int x, int y)
        {
            return Board[y, x];
        }

        public Domain.Table GetTableData()
        {
            return TableData;
        }

        public void AddShipsCellsCount()
        {
            TableData.CellsToShoot++;
        }
        
        public void RemoveBoard()
        {
            Board = new ECellState[TableData.Length, TableData.Width];
        }
    }
}