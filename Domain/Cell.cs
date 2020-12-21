#nullable enable

namespace Domain
{
    public class Cell
    {
        public int? CellId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int? GameStateIndex { get; set; }
        public int BombShipBoth { get; set; } // 0 - bomb , 1 - ship , 2 - bomb&ship
        public int GameStateId { get; set; }
        public GameState GameState { get; set; } = null!;
    }
}