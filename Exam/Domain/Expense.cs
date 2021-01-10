using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Expense
    {
        public int ExpenseId { get; set; }
        
        public int Price { get; set; }
        
        [MaxLength(4096,  ErrorMessage = "Description length is too long!")]
        public string? Description { get; set; }
        
        public DateTime Date { get; set; }
        
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        
        public int CarId { get; set; }
        public Car? Car { get; set; }
    }
}