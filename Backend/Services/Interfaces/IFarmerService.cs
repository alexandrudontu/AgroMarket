using Backend.DTOs.Farmer;

namespace Backend.Services.Interfaces
{
    public interface IFarmerService
    {
        Task<List<FarmerNearbyDto>> GetNearbyFarmersAsync(
            double latitude,
            double longitude,
            double maxDistanceKm
        );

        Task<FarmerProfileDto> GetFarmerProfileAsync(
            string farmerId,
            double? latitude,
            double? longitude
        );
    }
}