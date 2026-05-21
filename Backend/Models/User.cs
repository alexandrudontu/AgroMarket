using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Backend.Models
{
    public class User : IdentityUser
    {

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        public string? City { get; set; }

        public string? County { get; set; }

        public string? Address { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public ICollection<Product> ? Products { get; set; }
    }
}
