using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // Navigation property for related products
        public ICollection<Product> ? Products { get; set; }
    }
}
