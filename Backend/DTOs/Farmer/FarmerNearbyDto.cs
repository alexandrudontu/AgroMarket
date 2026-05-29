namespace Backend.DTOs.Farmer
{
    public class FarmerNearbyDto
    {
        public string Id { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? City { get; set; }

        public string? County { get; set; }

        public double DistanceKm { get; set; }

        public int ProductsCount { get; set; }

        public int SoldProductsCount { get; set; }
    }
}