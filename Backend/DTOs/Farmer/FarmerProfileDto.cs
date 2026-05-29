namespace Backend.DTOs.Farmer
{
    public class FarmerProfileDto
    {
        public string Id { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? City { get; set; }

        public string? County { get; set; }

        public string? Address { get; set; }

        public double? DistanceKm { get; set; }

        public int ProductsCount { get; set; }

        public int SoldProductsCount { get; set; }

        public decimal TotalRevenue { get; set; }

        public List<FarmerProductDto> Products { get; set; } = new();
    }
}