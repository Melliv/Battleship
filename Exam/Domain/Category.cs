using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Category
    {
        public int CategoryId { get; set; }
        [MaxLength(64,  ErrorMessage = "Category name is too long!")]
        public string Name { get; set; } = null!;
        
        public ICollection<Expense>? Expenses { get; set; }
    }
}