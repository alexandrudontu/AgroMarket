using Backend.DTOs.Orders;

namespace Backend.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto);
        Task<List<FarmerOrderDto>> GetFarmerOrdersAsync();
        Task<int> CheckoutAsync(CheckoutDto dto);
    }
}
