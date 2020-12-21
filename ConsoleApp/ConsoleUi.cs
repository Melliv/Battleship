using System;
using System.Linq;
using System.Text;
using GameBrain;
using GameBrain.Enums;

namespace ConsoleApp
{
    public class ConsoleUi
    {
        
        private readonly char[] _alphabet = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i',
                'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};
        public void DisplayBoards(Battleship battleship)
        {
            var width = battleship.GetGameData().Width;
            var length = battleship.GetGameData().Length;
            Table table1 = battleship.GetTable(0);
            Table table2 = battleship.GetTable(1);

            Console.WriteLine("");
            string spaces = "";
            for (var i = 0; i < width; i++)
            {
                spaces += "    ";
            }
            Console.WriteLine($"          {table1.GetTableData().PlayerName}{spaces}{table2.GetTableData().PlayerName}");
            
            StringBuilder sb = new StringBuilder("          --");
            for (var i = 0; i < width; i++)
            {
                sb.Append($"-{char.ToUpper(_alphabet[i])}-");
            }
            sb.Append("--");
            string alpRow = sb.ToString() + sb.ToString();
            Console.WriteLine(alpRow);
            
            DrawBoards(width, length, table1, table2);
            
            Console.WriteLine(alpRow);
        }

        private static void DrawBoards(int width, int length, Table table1, Table table2)
        {
            for (var rowIndex = 0; rowIndex < length; rowIndex++)
            {
                for (var tableIndex = 0; tableIndex < 2; tableIndex++)
                {
                    Table observableTable = tableIndex == 0 ? table1 : table2;
                    var rowNum = rowIndex + 1;
                    if (rowNum < 10)
                    {
                        Console.Write(" ");
                    }
                    Console.Write($"       {rowNum} | "); // 8 spaces
                    for (var cellIndex = 0; cellIndex < width; cellIndex++)
                    {
                        var cell = observableTable.GetCell(cellIndex, rowIndex);
                        var blindTable = observableTable.GetTableData().BlindTable;
                        Console.Write(" ");
                        DrawCell(cell, blindTable);
                        Console.Write(" ");
                    }
                    Console.Write(" |");
                }
                Console.WriteLine("");
            }
        }

        private static void DrawCell(ECellState cell, bool blindTable)
        {
            switch (cell)
            {
                case ECellState.Ship:
                    if (blindTable)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray; 
                        Console.Write("?");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green; 
                        Console.Write((char) 9632);
                    }
                    break;
                case ECellState.Empty:
                    if (blindTable)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray; 
                        Console.Write("?");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue; 
                        Console.Write((char) 9619);
                    }
                    break;
                case ECellState.Bomb:
                    Console.ForegroundColor = ConsoleColor.Yellow; 
                    Console.Write("X");
                    break;
                case ECellState.ShipAndBomb:
                    Console.ForegroundColor = ConsoleColor.Red; 
                    Console.Write("X");
                    break;
                default:
                    throw new ArgumentException("cell state is invalid!");
            }
            Console.ResetColor();
        }
        

        public (int, int) AskTableSize()
        {
            do
            {
                Console.WriteLine("Choose table size");
                Console.WriteLine("Dimensions must be within range 3 - 26! (example: 3,12)");
                var input = Console.ReadLine() ?? "";
                var dimensions = input.Split(",");
                try
                {
                    var xValue = Convert.ToInt32(dimensions[0]);
                    var yValue = Convert.ToInt32(dimensions[1]);
                    if (xValue < 3 || xValue > 26 )
                    {
                        DisplayError("Width value is not in the range!");              
                        continue;
                    }
                    if (yValue < 3 || yValue > 26)
                    {
                        DisplayError("Length value is not in the range!");
                        continue;
                    }
                    return (xValue, yValue);
                }
                catch (IndexOutOfRangeException)
                {
                    DisplayError("Invalid input!");
                }
                catch (FormatException)
                {
                    DisplayError("Invalid input!");
                }
            } while (true);
        }

        public string AskGameName()
        {
            while (true)
            {
                Console.WriteLine("Write game name (1-64)");
                Console.WriteLine("Must have 1-64 symbols!");
                Console.Write(">");
                var userChoice = Console.ReadLine();
                if (!string.IsNullOrEmpty(userChoice) && userChoice.Length <= 64)
                {
                    return userChoice;
                }
                DisplayError("Game name length is not in range!");
            }
        }

        public (string, string) AskPlayersName(int playerCount)
        {
            string[] playerNames = {"Computer1", "Computer2"};
            for (var i = 0; i < playerCount; i++)
            {
                while (true)
                {
                    Console.WriteLine($"Write Player{playerCount} name");
                    Console.WriteLine("Must have 1-16 symbols!");
                    Console.Write(">");
                    var userChoice = Console.ReadLine() ?? "";
                    if (userChoice.Length > 0 && userChoice.Length < 16)
                    {
                        playerNames[i] = userChoice;
                        break;
                    }
                    DisplayError("Player name length is not in range!");
                }
            }
            return (playerNames[0], playerNames[1]);
        }

        public bool AskShipPosSet()
        {
            while (true)
            {
                Console.WriteLine("Do you want to automated ship positioning (YES/NO)");
                Console.Write(">");
                var userChoice = Console.ReadLine()?.ToLower().Trim() ?? "";
                switch (userChoice)
                {
                    case "y":
                    case "yes":
                        return true;
                    case "n":
                    case "no":
                        return false;
                    default:
                        DisplayError("Input is invalid!");
                        break;
                }
            }
        }
        
        public void DisplayNotification(string notification)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("");
            Console.WriteLine(notification);
            Console.ResetColor();
        }

        public void DisplayError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("");
            Console.WriteLine(error);
            Console.ResetColor();
        }

        public (int, int) AskShootCell(string player, int width, int height)
        {
            do
            {
                Console.WriteLine($"{player} - Where do you want to shoot? (example: 'A12')");
                Console.Write(">");
                var userChoice = Console.ReadLine()?.ToLower().Trim() ?? "";
                
                var x = ValidateLetter(width, userChoice.Substring(0, 1));
                var y = ValidateNumber(height, userChoice.Substring(1, userChoice.Length - 1));
                
                if (x != -1 && y != -1)
                {
                    return (x, y);
                }
                if (x == -1)
                {
                    DisplayError("Input letter does not match!");
                }
                if (y == -1)
                {
                    DisplayError("Input number does not match!");
                }
            } while (true);
        }

        private int ValidateLetter(int allowedWidth, string input)
        {
            try
            {
                var inputLetter = char.Parse(input);
                if (!_alphabet.Contains(inputLetter)) return -1;
                
                var letterIndex = Array.FindIndex(_alphabet, letter => letter == inputLetter);
                
                if (letterIndex < allowedWidth) return letterIndex;
                
                return -1;
            }
            catch (ArgumentOutOfRangeException)
            {
                return -1;
            }

        }
        
        private static int ValidateNumber(int maxLength, string input)
        {
            try
            {
                return (int.TryParse(input, out var inputNumber) 
                        && inputNumber - 1 < maxLength && inputNumber > 0)
                    ? inputNumber - 1
                    : -1;
            }
            catch (ArgumentOutOfRangeException)
            {
                return -1;
            }

        }
        
    }
}