namespace Backend.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public string CustomerId { get; set; }

        public User Customer { get; set; }

        public ICollection<CartItem> ? CartItems { get; set; }
    }
}
