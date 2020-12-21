#nullable enable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Table
    {
        public int TableId { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
        public int CellsToShoot { get; set; }
        public bool BlindTable { get; set; }
        
        [MaxLength(20)]
        public string? PlayerName { get; set; }
        public ICollection<DefaultShip>? ShipsToPlace { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; } = null!;
    }
}