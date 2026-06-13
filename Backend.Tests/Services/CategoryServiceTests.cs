using Backend.DTOs.Categories;
using Backend.Models;
using Backend.Services.Implementations;
using Backend.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Backend.Tests.Services;

public class CategoryServiceTests
{
    [Fact]
    public async Task CreateAsync_persists_name_and_icon()
    {
        await using var context = TestDbContextFactory.Create();
        var service = new CategoryService(context);

        var result = await service.CreateAsync(new CreateCategoryDto
        {
            Name = "Fructe",
            Icon = "🍎"
        });

        Assert.NotEqual(0, result.Id);
        Assert.Equal("Fructe", result.Name);
        Assert.Equal("🍎", result.Icon);

        var savedCategory = await context.Categories.SingleAsync();
        Assert.Equal("Fructe", savedCategory.Name);
        Assert.Equal("🍎", savedCategory.Icon);
    }

    [Fact]
    public async Task GetAllAsync_returns_product_count_for_each_category()
    {
        await using var context = TestDbContextFactory.Create();

        var farmer = TestData.User("farmer-1");
        var vegetables = TestData.Category("Legume", "🥕");
        var fruits = TestData.Category("Fructe", "🍎");

        context.Users.Add(farmer);
        context.Categories.AddRange(vegetables, fruits);
        context.Products.AddRange(
            TestData.Product("Roșii", 8m, 20, vegetables, farmer),
            TestData.Product("Castraveți", 6m, 15, vegetables, farmer),
            TestData.Product("Mere", 5m, 30, fruits, farmer)
        );
        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Single(c => c.Name == "Legume").ProductsCount);
        Assert.Equal(1, result.Single(c => c.Name == "Fructe").ProductsCount);
    }

    [Fact]
    public async Task GetByIdAsync_when_category_does_not_exist_throws()
    {
        await using var context = TestDbContextFactory.Create();
        var service = new CategoryService(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetByIdAsync(999));
    }
}
