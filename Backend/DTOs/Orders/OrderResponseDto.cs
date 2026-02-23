namespace Backend.DTOs.Orders
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDetailsDto> Items { get; set; }
    }

    public class OrderItemDetailsDto
    {
        public string ProductName { get; set; }
        public string UnitOfMeasurement { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
    }
}
