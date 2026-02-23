using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Products
{
    public class UpdateProductDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string UnitOfMeasurement { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
