using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Images
{
    public class AddProductImageDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }
}