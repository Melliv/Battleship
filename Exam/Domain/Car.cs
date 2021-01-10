using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Domain
{
    public class Car
    {
        public int CarId { get; set; }

        [Display(Name="Number Plate")]
        [MaxLength(10)]
        public string? NumberPlate { get; set; }
        
        [Display(Name="Release year")]
        [Range(1900, 2100, ErrorMessage = "Be realistic!")]
        public int ReleaseYear { get; set; }
        
        [Display(Name="Reached date")]
        public DateTime ReachedDate { get; set; }
        
        //public string CreateDate { get; set; } = DateTime.Now.ToString("d", new CultureInfo("et-EE"));
        [MaxLength(64, ErrorMessage = "Car Mark name is too long!")]
        public string Mark  { get; set; } = null!;
        
        [MaxLength(64, ErrorMessage = "Car model name is too long!")]
        public string Model { get; set; } = null!;
        
        [Range(0, Double.MaxValue ,ErrorMessage = "Mileage cannot be negative!")]
        public int Mileage { get; set; }
        
        [MaxLength(64, ErrorMessage = "Gearbox name is too long!")]
        public string? Gearbox { get; set; }
        
        [MaxLength(64, ErrorMessage = "Fuel type name is too long!")]
        public string? FuelType { get; set; }

        [Display(Name="Market price")]
        public int MarketPrice { get; set; }
        
        [MaxLength(4096,  ErrorMessage = "History description length is too long!")]
        public string? History { get; set; }
        
        [Display(Name="Car type")]
        public int CarTypeId { get; set; }
        
        [Display(Name="Car type")]
        public CarType? CarType { get; set; }
        
        public ICollection<Expense>? Expenses { get; set; }
    }
}