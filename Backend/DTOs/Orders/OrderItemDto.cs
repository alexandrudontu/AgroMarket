using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Orders
{
    public class OrderItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }
    }
}
