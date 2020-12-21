using System;
using System.Collections.Generic;
using ConsoleApp.Exceptions;

namespace ConsoleApp
{
    public enum MenuLevel
    {
        Level0,
        Level1,
        Level2Plus 
    }
    
    public class Menu
    {
        private Dictionary<string, MenuItem> MenuItems { get; set; } = new Dictionary<string, MenuItem>();
        private readonly MenuLevel _menuLevel;

        public Menu(MenuLevel level)
        {
            _menuLevel = level;
        }

        public void AddMenuItem(MenuItem item)
        {
            if (item.UserChoice == "")
            {
                throw new ArgumentException("UserChoice cannot be empty");
            }
            
            foreach (var menuItem in MenuItems)
            {
                if (menuItem.Key == item.UserChoice)
                {
                    throw new ArgumentException($"This UserChoice {item.UserChoice} is already listed in the menu");
                }
            }
            MenuItems.Add(item.UserChoice, item);
        }
        public void RunMenu()
        {
            AddMenuEnd();
            string userChoice;
            do
            {
                foreach (var menuItem in MenuItems)
                {
                    Console.WriteLine(menuItem.Value);
                }

                Console.Write(">");
                userChoice = Console.ReadLine()?.ToLower().Trim() ?? "";
                
                if (MenuItems.TryGetValue(userChoice, out MenuItem? userMenuItem))
                {
                    try
                    {
                        userMenuItem.MethodToExecute();
                    }
                    catch (ReturnToPreviousException)
                    {
                        break;
                    }
                    catch (ReturnToMenuException)
                    {
                        if (_menuLevel != MenuLevel.Level0)
                        {
                            throw;
                        }
                    }
                    catch (ExitGameException)
                    {
                        if (_menuLevel != MenuLevel.Level0)
                        {
                            throw;
                        }
                        userChoice = "x";
                        Console.WriteLine("Closing down......");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("");
                    Console.WriteLine("I don't have this option!");
                    Console.ResetColor();
                }
            } while (userChoice != "x");
            
        }

        private void AddMenuEnd()
        {
            var exit = new MenuItem("Exit", "x", () => throw new ExitGameException());
            MenuItems.Add(exit.UserChoice, exit);
            if (_menuLevel == MenuLevel.Level0) return;
            var menu = new MenuItem("Menu", "m", () => throw new ReturnToMenuException());
            MenuItems.Add(menu.UserChoice, menu);
            if (_menuLevel == MenuLevel.Level1) return;
            var previous = new MenuItem("Previous", "p", () => throw new ReturnToPreviousException());
            MenuItems.Add(previous.UserChoice, previous);
        }
        
    }
}