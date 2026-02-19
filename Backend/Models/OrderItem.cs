using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        // Foreign key to Product
        public int ProductId { get; set; }

        public Product Product { get; set; }

        // Foreign key to Order
        public int OrderId { get; set; }

        public Order Order { get; set; }
    }
}
