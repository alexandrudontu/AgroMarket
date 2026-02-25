using Backend.Data;
using Backend.DTOs.Products;
using Backend.DTOs.Images;
using Backend.Models;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        public ProductService(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }
        public async Task<List<ProductListDto>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    UnitOfMeasurement = p.UnitOfMeasurement,
                    CategoryName = p.Category.Name
                })
                .ToListAsync();
        }
        public async Task<ProductDetailsDto> GetByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Farmer)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new Exception("Product not found");

            return new ProductDetailsDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Quantity = product.Quantity,
                Price = product.Price,
                UnitOfMeasurement = product.UnitOfMeasurement,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                FarmerName = product.Farmer.FirstName + " " + product.Farmer.LastName,
                Images = product.Images
                    .Select(i => new ProductImageDto
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl
                    }).ToList()
            };
        }
        public async Task<ProductDetailsDto> CreateAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                UnitOfMeasurement = dto.UnitOfMeasurement,
                CategoryId = dto.CategoryId,
                FarmerId = _currentUser.UserId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(product.Id);
        }
        public async Task UpdateAsync(UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(dto.Id);

            if (product == null)
                throw new Exception("Product not found");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.UnitOfMeasurement = dto.UnitOfMeasurement;
            product.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                throw new Exception("Product not found");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
