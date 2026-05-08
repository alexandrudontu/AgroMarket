using Backend.Data;
using Backend.DTOs;
using Backend.DTOs.Farmer;
using Backend.DTOs.Products;
using Backend.Models;
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

        public UsersController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context; ;
            _userManager = userManager;
        }

        [HttpGet("farmers")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<FarmerDto>>> GetFarmers()
        {
            var farmers = await _userManager.GetUsersInRoleAsync("Farmer");

            var result = farmers.Select(f => new FarmerDto
            {
                Id = f.Id,
                FirstName = f.FirstName,
                LastName = f.LastName,
                Email = f.Email
            });

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
    }
}