using Backend.DTOs.Cart;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
            => Ok(await _service.GetCartAsync());

        [HttpPost]
        public async Task<IActionResult> Add(AddToCartDto dto)
        {
            await _service.AddToCartAsync(dto);
            return Ok();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Remove(int productId)
        {
            await _service.RemoveFromCartAsync(productId);
            return NoContent();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> Clear()
        {
            await _service.ClearCartAsync();
            return NoContent();
        }
    }
}
