using Backend.Data;
using Backend.DTOs.Images;
using Backend.DTOs.Products;
using Backend.Models;
using Backend.Services.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly Cloudinary _cloudinary;

        public ProductService(ApplicationDbContext context, ICurrentUserService currentUser, Cloudinary cloudinary)
        {
            _context = context;
            _currentUser = currentUser;
            _cloudinary = cloudinary;
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

            return await query.Take(20).Select(p => new ProductListDto
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
                FarmerId = product.Farmer.Id,
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
                .FirstOrDefaultAsync(p => p.Id == dto.ProductId);

            if (product == null)
                throw new Exception("Product not found");

            if (product.FarmerId != _currentUser.UserId)
                throw new UnauthorizedAccessException("You can only upload images for your own products");

            if (dto.File == null || dto.File.Length == 0)
                throw new Exception("Invalid image file");

            await using var stream = dto.File.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(dto.File.FileName, stream),
                Folder = "agromarket/products",
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }

            var image = new ProductImages
            {
                ProductId = product.Id,
                ImageUrl = uploadResult.SecureUrl.ToString(),
                PublicId = uploadResult.PublicId
            };

            _context.ProductImages.Add(image);

            await _context.SaveChangesAsync();

            return new ProductImageDto
            {
                Id = image.Id,
                ImageUrl = image.ImageUrl,
                PublicId = image.PublicId
            };
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var image = await _context.ProductImages
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == imageId);

            if (image == null)
                throw new Exception("Image not found");

            if (image.Product.FarmerId != _currentUser.UserId)
                throw new UnauthorizedAccessException("You can only delete images from your own products");

            if (!string.IsNullOrWhiteSpace(image.PublicId))
            {
                var deleteParams = new DeletionParams(image.PublicId);

                var result = await _cloudinary.DestroyAsync(deleteParams);

                if (result.Error != null)
                {
                    throw new Exception(result.Error.Message);
                }
            }

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

        public async Task<List<ProductListDto>> GetNearbyProductsAsync(
            double userLat,double userLng, double maxDistanceKm,
            int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Farmer)
                .Where(p =>
                    p.Quantity > 0 &&
                    p.Farmer.Latitude != null &&
                    p.Farmer.Longitude != null)
                .AsQueryable();

            if (categoryId != null)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            if (minPrice != null)
            {
                query = query.Where(p => p.Price >= minPrice);
            }

            if (maxPrice != null)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }

            var products = await query.ToListAsync();

            var result = products
                .Select(p =>
                {
                    var distance = CalculateDistanceKm(
                        userLat,
                        userLng,
                        p.Farmer.Latitude!.Value,
                        p.Farmer.Longitude!.Value
                    );

                    return new ProductListDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Quantity = p.Quantity,
                        UnitOfMeasurement = p.UnitOfMeasurement,

                        CategoryId = p.CategoryId,
                        CategoryName = p.Category.Name,

                        FarmerId = p.FarmerId,
                        FarmerCity = p.Farmer.City,
                        FarmerCounty = p.Farmer.County,

                        DistanceKm = Math.Round(distance, 2),

                        ProductImages = p.Images.Select(i => new ProductImageDto
                        {
                            Id = i.Id,
                            ImageUrl = i.ImageUrl
                        }).ToList()
                    };
                })
                .Where(p => p.DistanceKm <= maxDistanceKm)
                .OrderBy(p => p.DistanceKm)
                .ToList();

            return result;
        }

        private double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double earthRadiusKm = 6371;

            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) *
                Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) *
                Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadiusKm * c;
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
