namespace Backend.DTOs.Cart
{
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
