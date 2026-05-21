namespace Backend.Services.Interfaces
{
    public interface IGeocodingService
    {
        Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(
        string county,
        string city,
        string? address
    );
    }
}
