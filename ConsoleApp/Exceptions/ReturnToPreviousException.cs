using System;

namespace ConsoleApp.Exceptions
{
    public class ReturnToPreviousException : Exception
    {
        public ReturnToPreviousException()
        {
        }
        
        public ReturnToPreviousException(string message) : base(message)
        {
        }
    }
}