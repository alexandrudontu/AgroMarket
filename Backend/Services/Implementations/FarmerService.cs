using Backend.Data;
using Backend.DTOs.Farmer;
using Backend.DTOs.Images;
using Backend.Models;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace Backend.Services.Implementations
{
    public class FarmerService : IFarmerService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public FarmerService(
            ApplicationDbContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<FarmerNearbyDto>> GetNearbyFarmersAsync(
            double latitude,
            double longitude,
            double maxDistanceKm)
        {
            var farmersInRole = await _userManager.GetUsersInRoleAsync("Farmer");

            var farmerIds = farmersInRole
                .Select(f => f.Id)
                .ToList();

            var farmers = await _context.Users
                .Where(u =>
                    farmerIds.Contains(u.Id) &&
                    u.Latitude != null &&
                    u.Longitude != null)
                .ToListAsync();

            var productsCount = await _context.Products
                .Where(p => farmerIds.Contains(p.FarmerId))
                .GroupBy(p => p.FarmerId)
                .Select(g => new
                {
                    FarmerId = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var soldProducts = await _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => farmerIds.Contains(oi.Product.FarmerId))
                .GroupBy(oi => oi.Product.FarmerId)
                .Select(g => new
                {
                    FarmerId = g.Key,
                    SoldCount = g.Sum(x => x.Quantity)
                })
                .ToListAsync();

            var result = farmers
                .Select(f =>
                {
                    var distance = CalculateDistanceKm(
                        latitude,
                        longitude,
                        f.Latitude!.Value,
                        f.Longitude!.Value
                    );

                    return new FarmerNearbyDto
                    {
                        Id = f.Id,
                        FirstName = f.FirstName,
                        LastName = f.LastName,
                        City = f.City,
                        County = f.County,
                        DistanceKm = Math.Round(distance, 2),

                        ProductsCount = productsCount
                            .FirstOrDefault(x => x.FarmerId == f.Id)?.Count ?? 0,

                        SoldProductsCount = soldProducts
                            .FirstOrDefault(x => x.FarmerId == f.Id)?.SoldCount ?? 0
                    };
                })
                .Where(f => f.DistanceKm <= maxDistanceKm)
                .OrderBy(f => f.DistanceKm)
                .ToList();

            return result;
        }

        public async Task<FarmerProfileDto> GetFarmerProfileAsync(
            string farmerId,
            double? latitude,
            double? longitude)
        {
            var farmer = await _userManager.FindByIdAsync(farmerId);

            if (farmer == null)
                throw new Exception("Farmer not found");

            var isFarmer = await _userManager.IsInRoleAsync(farmer, "Farmer");

            if (!isFarmer)
                throw new Exception("User is not a farmer");

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.FarmerId == farmerId)
                .ToListAsync();

            var soldItems = await _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.Product.FarmerId == farmerId)
                .ToListAsync();

            double? distanceKm = null;

            if (
                latitude != null &&
                longitude != null &&
                farmer.Latitude != null &&
                farmer.Longitude != null)
            {
                distanceKm = Math.Round(
                    CalculateDistanceKm(
                        latitude.Value,
                        longitude.Value,
                        farmer.Latitude.Value,
                        farmer.Longitude.Value
                    ),
                    2
                );
            }

            var result = new FarmerProfileDto
            {
                Id = farmer.Id,
                FirstName = farmer.FirstName,
                LastName = farmer.LastName,
                Email = farmer.Email,
                City = farmer.City,
                County = farmer.County,
                Address = farmer.Address,
                DistanceKm = distanceKm,

                ProductsCount = products.Count,

                SoldProductsCount = soldItems.Sum(i => i.Quantity),

                TotalRevenue = soldItems.Sum(i => i.Quantity * i.UnitPrice),

                Products = products.Select(p => new FarmerProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    UnitOfMeasurement = p.UnitOfMeasurement,
                    CategoryName = p.Category != null ? p.Category.Name : null,

                    SoldQuantity = soldItems
                        .Where(i => i.ProductId == p.Id)
                        .Sum(i => i.Quantity),

                    ProductImages = p.Images.Select(i => new ProductImageDto
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl
                    }).ToList()
                }).ToList()
            };

            return result;
        }

        private double CalculateDistanceKm(
            double lat1,
            double lon1,
            double lat2,
            double lon2)
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