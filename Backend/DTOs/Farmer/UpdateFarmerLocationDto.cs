namespace Backend.DTOs.Farmer
{
    public class UpdateFarmerLocationDto
    {
        public string City { get; set; } = string.Empty;

        public string County { get; set; } = string.Empty;

        public string? Address { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }
}
