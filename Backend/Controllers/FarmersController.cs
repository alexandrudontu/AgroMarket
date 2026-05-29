using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/farmers")]
    public class FarmersController : ControllerBase
    {
        private readonly IFarmerService _farmerService;

        public FarmersController(IFarmerService farmerService)
        {
            _farmerService = farmerService;
        }

        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyFarmers(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double maxDistanceKm = 50)
        {
            var farmers = await _farmerService.GetNearbyFarmersAsync(
                latitude,
                longitude,
                maxDistanceKm
            );

            return Ok(farmers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFarmerProfile(
            string id,
            [FromQuery] double? latitude = null,
            [FromQuery] double? longitude = null)
        {
            var farmer = await _farmerService.GetFarmerProfileAsync(
                id,
                latitude,
                longitude
            );

            return Ok(farmer);
        }
    }
}