using Backend.DTOs.Orders;

namespace Backend.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto);
        Task<List<FarmerOrderDto>> GetFarmerOrdersAsync(string farmerId);
        Task<List<OrderResponseDto>> GetCustomerOrdersAsync();
        Task<int> CheckoutAsync(CheckoutDto dto);
    }
}
