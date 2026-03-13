using Backend.DTOs.Cart;

namespace Backend.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync();

        Task AddToCartAsync(AddToCartDto dto);

        Task RemoveFromCartAsync(int productId);

        Task ClearCartAsync();
    }
}
