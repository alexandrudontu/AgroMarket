using Backend.DTOs.Categories;

namespace Backend.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync();
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
        Task DeleteAsync(int id);
    }
}
