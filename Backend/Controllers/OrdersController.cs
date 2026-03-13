using Backend.DTOs.Orders;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
            => Ok(await _service.CreateOrderAsync(dto));


        [Authorize(Roles = "Farmer")]
        [HttpGet("farmer")]
        public async Task<IActionResult> GetFarmerOrders()
        {
            return Ok(await _service.GetFarmerOrdersAsync());
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(CheckoutDto dto)
        {
            var orderId = await _service.CheckoutAsync(dto);

            return Ok(new
            {
                message = "Order created successfully",
                orderId = orderId
            });
        }
    }
}
