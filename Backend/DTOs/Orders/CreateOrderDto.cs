using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Orders
{
    public class CreateOrderDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "Comanda trebuie să conțină cel puțin un produs.")]
        public List<OrderItemDto> Items { get; set; }
    }
}
