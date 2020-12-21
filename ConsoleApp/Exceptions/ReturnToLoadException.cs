using System;

namespace ConsoleApp.Exceptions
{
    public class ReturnToLoadException : Exception
    {
        
        public ReturnToLoadException(string message) : base(message)
        {
        }
    }
}