using Backend.DTOs.Products;

namespace Backend.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductListDto>> GetAllAsync();
        Task<ProductDetailsDto> GetByIdAsync(int id);
        Task<ProductDetailsDto> CreateAsync(CreateProductDto dto);
        Task UpdateAsync(UpdateProductDto dto);
        Task DeleteAsync(int id);
    }
}
