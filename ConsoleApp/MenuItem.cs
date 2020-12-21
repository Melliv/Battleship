using System;
using static System.Char;

namespace ConsoleApp
{
    
    public class MenuItem
    {
        private string Label { get; set; }
        public string UserChoice { get; set; }
        
        public Action MethodToExecute { get; set; }

        public MenuItem(string label, string userChoice, Action methodToExecute)
        {
            Label = label.Trim();
            UserChoice = userChoice;
            MethodToExecute = methodToExecute;
        }
        
        public override string ToString()
        {
            return MakeUserChoiceUpper(UserChoice) + ") " + Label;
        }

        private static string MakeUserChoiceUpper(string userChoice)
        {
            if (userChoice.Length != 1 || !TryParse(userChoice, out var userChoiceChar))
            {
                return userChoice;
            }
            return char.ToString(ToUpper(userChoiceChar));
        }
    }
}