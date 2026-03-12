namespace Backend.DTOs.Orders
{
    public class FarmerOrderItemDto
    {
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal LineTotal { get; set; }
    }
}
