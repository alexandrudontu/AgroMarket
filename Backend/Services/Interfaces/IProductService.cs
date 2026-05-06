using Backend.DTOs.Products;

namespace Backend.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductDetailsDto> GetByIdAsync(int id);
        Task<List<ProductListDto>> GetProductsAsync(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice);
        Task<List<ProductListDto>> GetMyProductsAsync();
        Task<ProductDetailsDto> CreateAsync(CreateProductDto dto);
        Task UpdateAsync(UpdateProductDto dto);
        Task DeleteAsync(int id);
    }
}
