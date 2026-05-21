using Backend.DTOs.Images;
using Backend.DTOs.Products;
using Backend.Models;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDetailsDto>>> GetProducts(
        [FromQuery] string? search,
        [FromQuery] int? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice)
        {
            var products = await _service.GetProductsAsync(search, categoryId, minPrice, maxPrice);
            return Ok(products);
        }

        [Authorize(Roles = "Farmer")]
        [HttpGet("my")]
        public async Task<ActionResult> GetMyProducts()
        {
            return Ok(await _service.GetMyProductsAsync());
        }

        [Authorize(Roles = "Farmer")]
        [HttpPost]
        public async Task<ActionResult> Create(CreateProductDto dto)
        {
            var product = await _service.CreateAsync(dto);
            return Ok(product);
        }

        [Authorize(Roles = "Farmer,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Farmer,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/images")]
        [Authorize(Roles = "Farmer,Admin")]
        public async Task<ActionResult> UploadImage(
        [FromForm] AddProductImageDto dto)
        {
            var image = await _service.UploadImageAsync(dto);
            return Ok(image);
        }

        [HttpDelete("images/{id}")]
        [Authorize(Roles = "Farmer,Admin")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            await _service.DeleteImageAsync(id);
            return NoContent();
        }

        [HttpPut("images/{id}/main")]
        public async Task<IActionResult> SetMain(int id)
        {
            await _service.SetMainImageAsync(id);
            return NoContent();
        }

        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyProducts(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double maxDistanceKm = 50,
            [FromQuery] int? categoryId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null)
        {
            var products = await _service.GetNearbyProductsAsync(
                latitude,
                longitude,
                maxDistanceKm,
                categoryId,
                minPrice,
                maxPrice
            );

            return Ok(products);
        }
    }
}
