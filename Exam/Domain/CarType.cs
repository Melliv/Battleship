using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class CarType
    {
        public int CarTypeId { get; set; }
        [MaxLength(64, ErrorMessage = "CarType name is too long!")]
        public string Name { get; set; } = null!;
        
        public ICollection<Car>? Cars { get; set; }
    }
}