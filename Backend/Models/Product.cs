using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        public string UnitOfMeasurement { get; set; }  // kg, liters, pieces etc.

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string Category { get; set; }  // Vegetables, Fruits, Dairy etc.

        // Foreign key to User (Farmer)
        public int FarmerId { get; set; }

        // Navigation property
        public User Farmer { get; set; }
    }
}
