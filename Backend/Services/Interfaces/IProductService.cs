using Backend.DTOs.Images;
using Backend.DTOs.Products;

namespace Backend.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductDetailsDto> GetByIdAsync(int id);
        Task<List<ProductListDto>> GetProductsAsync(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice);
        Task<List<ProductListDto>> GetMyProductsAsync();
        Task<ProductDetailsDto> CreateAsync(CreateProductDto dto);
        Task UpdateAsync(int id, UpdateProductDto dto);
        Task DeleteAsync(int id);
        Task<ProductImageDto> UploadImageAsync(AddProductImageDto dto);
        Task DeleteImageAsync(int imageId);
        Task SetMainImageAsync(int imageId);
        Task<List<ProductListDto>> GetNearbyProductsAsync(double userLat, double userLng, double maxDistanceKm,
            int? categoryId, decimal? minPrice, decimal? maxPrice);
    }
}
