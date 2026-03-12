using Backend.DTOs.Products;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            return Ok(await _service.GetByIdAsync(id));
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

        [Authorize(Roles = "Farmer")]
        [HttpPut]
        public async Task<IActionResult> Update(UpdateProductDto dto)
        {
            await _service.UpdateAsync(dto);
            return NoContent();
        }

        [Authorize(Roles = "Farmer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
