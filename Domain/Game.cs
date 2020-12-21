#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Game
    {
        public int GameId { get; set; }

        [MaxLength(20)]
        public string? GameName { get; set; }
        public bool PlayerOneTurn { get; set; } = true;
        public bool AutomatedShipsPos { get; set; } = true;
        public int BombsCount { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
        public string Date { get; set; } = DateTime.Now.ToLongDateString();
        public ICollection<Table>? Tables { get; set; }
        public ICollection<GameState>? GameStates { get; set; }
    }
}