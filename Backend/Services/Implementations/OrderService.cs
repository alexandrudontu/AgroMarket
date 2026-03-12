using Backend.Data;
using Backend.DTOs.Orders;
using Backend.Models;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Implementations
{
    public class OrderService : IOrderService

    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public OrderService(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                throw new Exception("Order must contain items");

            var productIds = dto.Items.Select(i => i.ProductId).ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            var order = new Order
            {
                CustomerId = _currentUser.UserId,
                OrderDate = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            decimal total = 0;

            foreach (var item in dto.Items)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);

                if (product == null)
                    throw new Exception($"Product {item.ProductId} not found");

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                total += item.Quantity * product.Price;

                order.OrderItems.Add(orderItem);
            }

            order.TotalAmount = total;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return new OrderResponseDto
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(i => new OrderItemDetailsDto
                {
                    ProductName = products.First(p => p.Id == i.ProductId).Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineTotal = i.Quantity * i.UnitPrice
                }).ToList()
            };
        }

        public async Task<List<FarmerOrderDto>> GetFarmerOrdersAsync()
        {
            var farmerId = _currentUser.UserId;

            var orders = await _context.OrderItems
                .Where(oi => oi.Product.FarmerId == farmerId)
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .ThenInclude(o => o.Customer)
                .ToListAsync();

            var grouped = orders
                .GroupBy(o => o.OrderId)
                .Select(g => new FarmerOrderDto
                {
                    OrderId = g.Key,
                    CustomerName = g.First().Order.Customer.FirstName + " " +
                                   g.First().Order.Customer.LastName,
                    OrderDate = g.First().Order.OrderDate,

                    Items = g.Select(i => new FarmerOrderItemDto
                    {
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        LineTotal = i.Quantity * i.UnitPrice
                    }).ToList()
                }).ToList();

            return grouped;
        }

    }
}
