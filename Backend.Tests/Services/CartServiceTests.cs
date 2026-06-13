using Backend.DTOs.Cart;
using Backend.Models;
using Backend.Services.Implementations;
using Backend.Tests.Fakes;
using Backend.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Backend.Tests.Services;

public class CartServiceTests
{
    [Fact]
    public async Task AddToCartAsync_adds_product_and_GetCartAsync_calculates_total()
    {
        await using var context = TestDbContextFactory.Create();
        var customer = TestData.User("customer-1");
        var farmer = TestData.User("farmer-1");
        var category = TestData.Category();
        var product = TestData.Product("Roșii", 8m, 50, category, farmer);
        product.Images!.Add(new ProductImages { ImageUrl = "rosii.jpg", Product = product });

        context.Users.AddRange(customer, farmer);
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new CartService(context, new FakeCurrentUserService(customer.Id));

        await service.AddToCartAsync(new AddToCartDto
        {
            ProductId = product.Id,
            Quantity = 3
        });

        var cart = await service.GetCartAsync();

        Assert.Single(cart.Items);
        Assert.Equal(24m, cart.TotalAmount);
        Assert.Equal("Roșii", cart.Items[0].ProductName);
        Assert.Equal(3, cart.Items[0].Quantity);
        Assert.Equal(8m, cart.Items[0].UnitPrice);
        Assert.Equal("rosii.jpg", cart.Items[0].ImageUrl);
    }

    [Fact]
    public async Task AddToCartAsync_when_product_already_exists_increments_quantity()
    {
        await using var context = TestDbContextFactory.Create();
        var customer = TestData.User("customer-1");
        var farmer = TestData.User("farmer-1");
        var category = TestData.Category();
        var product = TestData.Product("Cartofi", 4m, 100, category, farmer);

        context.Users.AddRange(customer, farmer);
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new CartService(context, new FakeCurrentUserService(customer.Id));

        await service.AddToCartAsync(new AddToCartDto { ProductId = product.Id, Quantity = 2 });
        await service.AddToCartAsync(new AddToCartDto { ProductId = product.Id, Quantity = 5 });

        var cart = await service.GetCartAsync();

        Assert.Single(cart.Items);
        Assert.Equal(7, cart.Items[0].Quantity);
        Assert.Equal(28m, cart.TotalAmount);
    }

    [Fact]
    public async Task UpdateQuantityAsync_with_zero_quantity_removes_item()
    {
        await using var context = TestDbContextFactory.Create();
        var customer = TestData.User("customer-1");
        var farmer = TestData.User("farmer-1");
        var category = TestData.Category();
        var product = TestData.Product("Ceapă", 3m, 40, category, farmer);

        context.Users.AddRange(customer, farmer);
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new CartService(context, new FakeCurrentUserService(customer.Id));

        await service.AddToCartAsync(new AddToCartDto { ProductId = product.Id, Quantity = 4 });
        await service.UpdateQuantityAsync(new UpdateCartItemDto { ProductId = product.Id, Quantity = 0 });

        var cart = await service.GetCartAsync();

        Assert.Empty(cart.Items);
        Assert.Equal(0m, cart.TotalAmount);
        Assert.Equal(0, await context.CartItems.CountAsync());
    }

    [Fact]
    public async Task AddToCartAsync_when_product_does_not_exist_throws()
    {
        await using var context = TestDbContextFactory.Create();
        var customer = TestData.User("customer-1");
        context.Users.Add(customer);
        await context.SaveChangesAsync();

        var service = new CartService(context, new FakeCurrentUserService(customer.Id));

        var exception = await Assert.ThrowsAsync<Exception>(() =>
            service.AddToCartAsync(new AddToCartDto { ProductId = 999, Quantity = 1 }));

        Assert.Equal("Product not found", exception.Message);
    }
}
