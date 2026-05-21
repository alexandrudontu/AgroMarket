using Backend.Data;
using Backend.DTOs.Orders;
using Backend.Enums;
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
                OrderItems = new List<OrderItem>(),
                Status = OrderStatus.Pending
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
                    UnitPrice = product.Price,
                    UnitOfMeasurement = product.UnitOfMeasurement
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
                Status = OrderStatus.Pending.ToString(),
                Items = order.OrderItems.Select(i => new OrderItemDetailsDto
                {
                    ProductName = products.First(p => p.Id == i.ProductId).Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    UnitOfMeasurement = i.UnitOfMeasurement,
                    LineTotal = i.Quantity * i.UnitPrice,
                    ImageUrl = i.ImageUrl
                }).ToList()
            };
        }

        public async Task<List<FarmerOrderDto>> GetFarmerOrdersAsync(string farmerId)
        {
            var orders = await _context.OrderItems

                .Where(oi =>
                    oi.Product.FarmerId == farmerId)

                .Include(oi => oi.Product)
                    .ThenInclude(p => p.Images)

                .Include(oi => oi.Order)
                    .ThenInclude(o => o.Customer)

                .ToListAsync();

            var grouped = orders

                .GroupBy(oi => oi.OrderId)

                .Select(g => new FarmerOrderDto
                {
                    OrderId = g.Key,

                    CustomerName =
                        g.First().Order.Customer.FirstName
                        + " " +
                        g.First().Order.Customer.LastName,

                    OrderDate =
                        g.First().Order.OrderDate,

                    TotalAmount =
                        g.Sum(i =>
                            i.Quantity * i.UnitPrice),

                    Items = g.Select(i =>
                        new FarmerOrderItemDto
                        {
                            ProductName =
                                i.Product.Name,

                            Quantity =
                                i.Quantity,

                            UnitPrice =
                                i.UnitPrice,

                            LineTotal =
                                i.Quantity * i.UnitPrice,

                            ImageUrl =
                                i.Product.Images
                                    .Select(img => img.ImageUrl)
                                    .FirstOrDefault()
                        }).ToList()
                })

                .OrderByDescending(o => o.OrderDate)

                .ToList();

            return grouped;
        }

        public Task<List<OrderResponseDto>> GetCustomerOrdersAsync()
        {
            var orders = _context.Orders
                .Where(o => o.CustomerId == _currentUser.UserId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Images)
                .ToList();

            var grouped = orders
                .GroupBy(o => o.OrderDate)
                .Select(g => new OrderResponseDto
                {
                    OrderId = g.First().Id,
                    OrderDate = g.Key,
                    Status = OrderStatus.Pending.ToString(),
                    TotalAmount = g.Sum(o => o.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)),
                    Items = g.SelectMany(o => o.OrderItems).Select(oi => new OrderItemDetailsDto
                    {
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        UnitOfMeasurement = oi.UnitOfMeasurement,
                        UnitPrice = oi.UnitPrice,
                        LineTotal = oi.Quantity * oi.UnitPrice,
                        ImageUrl = oi.ImageUrl
                    }).ToList()
                }).ToList();

            return Task.FromResult(grouped);
        }

        public async Task<int> CheckoutAsync(CheckoutDto dto)
        {
            var userId = _currentUser.UserId;

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.CustomerId == userId);

            if (cart == null || !cart.CartItems.Any())
                throw new Exception("Cart is empty");

            // check stock availability
            foreach (var item in cart.CartItems)
            {
                if (item.Product.Quantity < item.Quantity)
                {
                    throw new Exception(
                        $"Insufficient stock for {item.Product.Name}. " +
                        $"Available: {item.Product.Quantity}, requested: {item.Quantity}"
                    );
                }
            }

            var order = new Order
            {
                CustomerId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>()
            };

            // calculate total and create order items
            foreach (var item in cart.CartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price,
                    UnitOfMeasurement = item.Product.UnitOfMeasurement,
                    ImageUrl = item.Product.Images?.FirstOrDefault()?.ImageUrl
                });

                // reduce stock
                item.Product.Quantity -= item.Quantity;
            }

            _context.Orders.Add(order);

            //Clear cart
            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            return order.Id;
        }
    }
}
