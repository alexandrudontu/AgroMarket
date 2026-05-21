using Backend.Data;
using Backend.DTOs.Categories;
using Backend.Models;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = c.Icon,
                    ProductsCount = c.Products.Count
                })
                .ToListAsync();
        }
        public Task<CategoryDto> GetByIdAsync(int id)
        {
            return _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = c.Icon,
                })
                .FirstOrDefaultAsync() ?? throw new KeyNotFoundException("Category not found");
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Icon = category.Icon,
            };
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
