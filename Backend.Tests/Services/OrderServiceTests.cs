using Backend.DTOs.Orders;
using Backend.Models;
using Backend.Services.Implementations;
using Backend.Tests.Fakes;
using Backend.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Backend.Tests.Services;

public class OrderServiceTests
{
    [Fact]
    public async Task CheckoutAsync_creates_order_reduces_stock_copies_snapshot_data_and_clears_cart()
    {
        await using var context = TestDbContextFactory.Create();
        var customer = TestData.User("customer-1");
        var farmer = TestData.User("farmer-1");
        var category = TestData.Category();
        var product = TestData.Product("Mere", 5m, 10, category, farmer);
        product.Images!.Add(new ProductImages { ImageUrl = "mere.jpg", Product = product });

        context.Users.AddRange(customer, farmer);
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        context.Carts.Add(new Cart
        {
            CustomerId = customer.Id,
            CartItems = new List<CartItem>
            {
                new CartItem
                {
                    ProductId = product.Id,
                    Quantity = 3
                }
            }
        });
        await context.SaveChangesAsync();

        var service = new OrderService(context, new FakeCurrentUserService(customer.Id));

        var orderId = await service.CheckoutAsync(new CheckoutDto { DeliveryAddress = "Str. Test 1" });

        var order = await context.Orders
            .Include(o => o.OrderItems)
            .SingleAsync(o => o.Id == orderId);

        var orderItem = Assert.Single(order.OrderItems!);
        Assert.Equal(customer.Id, order.CustomerId);
        Assert.Equal(15m, order.TotalAmount);
        Assert.Equal(product.Id, orderItem.ProductId);
        Assert.Equal(3, orderItem.Quantity);
        Assert.Equal(5m, orderItem.UnitPrice);
        Assert.Equal("kg", orderItem.UnitOfMeasurement);
        Assert.Equal("mere.jpg", orderItem.ImageUrl);

        var updatedProduct = await context.Products.SingleAsync(p => p.Id == product.Id);
        Assert.Equal(7, updatedProduct.Quantity);
        Assert.Equal(0, await context.CartItems.CountAsync());
    }

    [Fact]
    public async Task CheckoutAsync_when_cart_is_empty_throws()
    {
        await using var context = TestDbContextFactory.Create();
        var customer = TestData.User("customer-1");
        context.Users.Add(customer);
        context.Carts.Add(new Cart { CustomerId = customer.Id, CartItems = new List<CartItem>() });
        await context.SaveChangesAsync();

        var service = new OrderService(context, new FakeCurrentUserService(customer.Id));

        var exception = await Assert.ThrowsAsync<Exception>(() =>
            service.CheckoutAsync(new CheckoutDto()));

        Assert.Equal("Cart is empty", exception.Message);
    }

    [Fact]
    public async Task CheckoutAsync_when_stock_is_insufficient_throws_and_does_not_create_order()
    {
        await using var context = TestDbContextFactory.Create();
        var customer = TestData.User("customer-1");
        var farmer = TestData.User("farmer-1");
        var category = TestData.Category();
        var product = TestData.Product("Căpșuni", 12m, 1, category, farmer);

        context.Users.AddRange(customer, farmer);
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        context.Carts.Add(new Cart
        {
            CustomerId = customer.Id,
            CartItems = new List<CartItem>
            {
                new CartItem { ProductId = product.Id, Quantity = 2 }
            }
        });
        await context.SaveChangesAsync();

        var service = new OrderService(context, new FakeCurrentUserService(customer.Id));

        var exception = await Assert.ThrowsAsync<Exception>(() =>
            service.CheckoutAsync(new CheckoutDto()));

        Assert.Contains("Insufficient stock", exception.Message);
        Assert.Equal(0, await context.Orders.CountAsync());
        Assert.Equal(1, (await context.Products.SingleAsync(p => p.Id == product.Id)).Quantity);
    }
}
