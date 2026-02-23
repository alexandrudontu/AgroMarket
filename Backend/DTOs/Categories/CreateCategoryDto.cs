using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Categories
{
    public class CreateCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
