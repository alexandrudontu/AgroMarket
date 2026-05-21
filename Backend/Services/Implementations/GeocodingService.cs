using Backend.Services.Interfaces;
using System.Globalization;
using System.Text.Json;

public class GeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;

    public GeocodingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(
        string county,
        string city,
        string? address)
    {
        var query = $"{address}, {city}, {county}, Romania";

        var url =
            "https://nominatim.openstreetmap.org/search" +
            $"?q={Uri.EscapeDataString(query)}" +
            "&format=json" +
            "&limit=1";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        var results = JsonSerializer.Deserialize<List<NominatimResult>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        var first = results?.FirstOrDefault();

        if (first == null)
            return null;

        var latitude = double.Parse(
            first.Lat,
            CultureInfo.InvariantCulture
        );

        var longitude = double.Parse(
            first.Lon,
            CultureInfo.InvariantCulture
        );

        return (latitude, longitude);
    }

    private class NominatimResult
    {
        public string Lat { get; set; } = string.Empty;

        public string Lon { get; set; } = string.Empty;
    }
}