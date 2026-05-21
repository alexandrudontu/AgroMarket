using Backend.Data;
using Backend.DTOs;
using Backend.DTOs.Farmer;
using Backend.DTOs.Images;
using Backend.DTOs.Orders;
using Backend.DTOs.Products;
using Backend.Models;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IGeocodingService _geocodingService;
        private readonly ICurrentUserService _currentUser;

        public UsersController(ApplicationDbContext context, UserManager<User> userManager, IGeocodingService geocodingService, ICurrentUserService currentUserService)
        {
            _context = context; ;
            _userManager = userManager;
            _geocodingService = geocodingService;
            _currentUser = currentUserService;
        }

        [HttpGet("farmers")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<FarmerDto>>> GetFarmers()
        {
            var farmers = await _userManager
                .GetUsersInRoleAsync("Farmer");

            var result = new List<FarmerDto>();

            foreach (var f in farmers)
            {
                // PRODUCTS
                var products = await _context.Products
                    .Where(p => p.FarmerId == f.Id)
                    .Include(p => p.Images)
                    .Include(p => p.Category)
                    .ToListAsync();

                // ORDERS containing farmer products
                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Include(o => o.Customer)
                    .Where(o => o.OrderItems
                        .Any(oi => oi.Product.FarmerId == f.Id))
                    .ToListAsync();

                result.Add(new FarmerDto
                {
                    Id = f.Id,

                    FirstName = f.FirstName,

                    LastName = f.LastName,

                    Email = f.Email,

                    ProductsCount = products.Count,

                    OrdersCount = orders.Count,

                    Products = products.Select(p => new ProductListDto
                    {
                        Id = p.Id,

                        Name = p.Name,

                        Description = p.Description,

                        Price = p.Price,

                        Quantity = p.Quantity,

                        UnitOfMeasurement = p.UnitOfMeasurement,

                        CategoryId = p.CategoryId,

                        CategoryName = p.Category.Name,

                        ProductImages = p.Images
                            .Select(i => new ProductImageDto
                            {
                                Id = i.Id,
                                ImageUrl = i.ImageUrl,
                                IsMain = i.IsMain
                            })
                            .ToList()

                    }).ToList(),

                    Orders = orders.Select(o => new OrderResponseDto
                    {
                        OrderId = o.Id,

                        TotalAmount = o.TotalAmount,

                        OrderDate = o.OrderDate,

                        CustomerName =
                            o.Customer.FirstName + " " +
                            o.Customer.LastName

                    }).ToList()
                });
            }

            return Ok(result);
        }

        [HttpGet("farmers/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<FarmerDetailsDto>> GetFarmer(string id)
        {
            var farmer = await _userManager.FindByIdAsync(id);

            if (farmer == null)
                return NotFound();

            var isFarmer = await _userManager.IsInRoleAsync(farmer, "Farmer");

            if (!isFarmer)
                return BadRequest("User is not a farmer");

            var dto = new FarmerDetailsDto
            {
                Id = farmer.Id,
                FirstName = farmer.FirstName,
                LastName = farmer.LastName,
                Email = farmer.Email,
                ProductsCount = await _context.Products
                    .CountAsync(p => p.FarmerId == farmer.Id)
            };

            return Ok(dto);
        }

        [HttpGet("farmers/{id}/products")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProductListDto>>> GetFarmerProducts(string id)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.FarmerId == id)
                .Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name
                })
                .ToListAsync();

            return Ok(products);
        }

        [Authorize(Roles = "Farmer")]
        [HttpPut("farmer/location")]
        public async Task<IActionResult> UpdateFarmerLocation([FromBody] UpdateFarmerLocationDto dto)
        {
            var farmer = await _userManager.FindByIdAsync(
                _currentUser.UserId
            );

            if (farmer == null)
                return NotFound("Fermierul nu a fost găsit.");

            if (string.IsNullOrWhiteSpace(dto.County))
                return BadRequest("Județul este obligatoriu.");

            if (string.IsNullOrWhiteSpace(dto.City))
                return BadRequest("Localitatea este obligatorie.");


            double? latitude = dto.Latitude;
            double? longitude = dto.Longitude;

            if (latitude == null || longitude == null)
            {
                var coordinates =
                    await _geocodingService.GetCoordinatesAsync(
                        dto.County,
                        dto.City,
                        dto.Address
                    );

                if (coordinates == null)
                {
                    return BadRequest(
                        "Nu am putut găsi coordonatele pentru această locație."
                    );
                }

                latitude = coordinates.Value.Latitude;
                longitude = coordinates.Value.Longitude;
            }

            farmer.County = dto.County;
            farmer.City = dto.City;
            farmer.Address = dto.Address;
            farmer.Latitude = latitude;
            farmer.Longitude = longitude;

            await _userManager.UpdateAsync(farmer);

            return NoContent();
        }
    }
}