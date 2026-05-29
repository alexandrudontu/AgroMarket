using Backend.DTOs.Images;

namespace Backend.DTOs.Farmer
{
    public class FarmerProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string UnitOfMeasurement { get; set; } = string.Empty;

        public string? CategoryName { get; set; }

        public int SoldQuantity { get; set; }

        public List<ProductImageDto> ProductImages { get; set; } = new();
    }
}