using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class ProductImages
    {
        public int Id { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        // Foreign key to Product
        public int ProductId { get; set; }

        // Navigation property
        public Product Product { get; set; }
    }
}
