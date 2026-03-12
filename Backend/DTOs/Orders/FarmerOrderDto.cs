namespace Backend.DTOs.Orders
{
    public class FarmerOrderDto
    {
        public int OrderId { get; set; }

        public string CustomerName { get; set; }

        public DateTime OrderDate { get; set; }

        public List<FarmerOrderItemDto> Items { get; set; }
    }
}
