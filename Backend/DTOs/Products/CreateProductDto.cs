using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Products
{
    public class CreateProductDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public string UnitOfMeasurement { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
