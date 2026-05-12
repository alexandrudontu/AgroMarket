using Backend.Data;
using Backend.DTOs.Images;
using Backend.DTOs.Products;
using Backend.Models;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<List<ProductListDto>> GetProductsAsync(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products.Include(p => p.Images).Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p =>
                    EF.Functions.Like(p.Name, $"%{search}%"));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            return await query.Take(10).Select(p => new ProductListDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Quantity = p.Quantity,
                Price = p.Price,
                UnitOfMeasurement = p.UnitOfMeasurement,
                CategoryName = p.Category.Name,
                ProductImages = p.Images.Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl
                }).ToList()
            }).ToListAsync();
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
        public async Task<List<ProductListDto>> GetMyProductsAsync()
        {
            return await _context.Products
                .Where(p => p.FarmerId == _currentUser.UserId)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    UnitOfMeasurement = p.UnitOfMeasurement,
                    Quantity = p.Quantity,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    ProductImages = p.Images
                        .Select(i => new ProductImageDto
                        {
                            Id = i.Id,
                            ImageUrl = i.ImageUrl
                        })
                        .ToList()
                })
                .ToListAsync();
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
        public async Task UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                throw new Exception("Product not found");

            if (product.FarmerId != _currentUser.UserId)
            {
                throw new UnauthorizedAccessException(
                    "You can only update your own products");
            }

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Quantity = dto.Quantity;
            product.UnitOfMeasurement = dto.UnitOfMeasurement;
            product.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                throw new Exception("Product not found");

            if (product.FarmerId != _currentUser.UserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own products");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<ProductImageDto> UploadImageAsync(AddProductImageDto dto)
        {
            var product = await _context.Products
                .FindAsync(dto.ProductId);


            // folder
            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/Images");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // unique filename
            var fileName = Guid.NewGuid() +
                           Path.GetExtension(dto.File.FileName);

            var filePath = Path.Combine(folderPath, fileName);

            // save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            // save in db
            var image = new ProductImages
            {
                ProductId = dto.ProductId,
                ImageUrl = $"/images/{fileName}"
            };

            _context.ProductImages.Add(image);

            await _context.SaveChangesAsync();

            return new ProductImageDto
            {
                Id = image.Id,
                ImageUrl = image.ImageUrl
            };
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);

            if (image == null)
                throw new Exception("Image not found");

            _context.ProductImages.Remove(image);

            await _context.SaveChangesAsync();
        }

        public async Task SetMainImageAsync(int imageId)
        {
            var image = await _context.ProductImages
                .Include(i => i.Product)
                .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(i => i.Id == imageId);

            if (image == null)
                throw new Exception("Image not found");

            foreach (var img in image.Product.Images)
            {
                img.IsMain = false;
            }

            image.IsMain = true;

            await _context.SaveChangesAsync();
        }
    }
}
