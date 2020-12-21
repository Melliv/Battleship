using System.Collections.Generic;

namespace Domain
{
    public class GameState
    {
        public int GameStateId { get; set; }
        
        public int TableIndex { get; set; }
        public ICollection<Cell> ActiveCells { get; set; } = null!;
        public int GameStateIndex { get; set; } // board ship index
        public int GameId { get; set; }
        public Game Game { get; set; } = null!;
    }
    
}