namespace Backend.DTOs.Cart
{
    public class CartItemDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public string UnitOfMeasurement { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public decimal LineTotal { get; set; }

        public string? ImageUrl { get; set; }
    }
}
