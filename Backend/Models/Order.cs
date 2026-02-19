using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public decimal TotalAmount { get; set; }

        // Foreign key to Buyer (User)
        public int BuyerId { get; set; }

        // Navigation property
        public User Buyer { get; set; }

        public ICollection<OrderItem> ? OrderItems { get; set; }
    }
}
