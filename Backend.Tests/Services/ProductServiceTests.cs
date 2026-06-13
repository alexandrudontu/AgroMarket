using Backend.DTOs.Products;
using Backend.Models;
using Backend.Services.Implementations;
using Backend.Tests.Fakes;
using Backend.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Backend.Tests.Services;

public class ProductServiceTests
{
    [Fact]
    public async Task UpdateAsync_when_product_belongs_to_current_farmer_updates_product()
    {
        await using var context = TestDbContextFactory.Create();
        var farmer = TestData.User("farmer-1");
        var category = TestData.Category("Legume", "🥕");
        var newCategory = TestData.Category("Fructe", "🍎");
        var product = TestData.Product("Roșii", 8m, 20, category, farmer);

        context.Users.Add(farmer);
        context.Categories.AddRange(category, newCategory);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context, new FakeCurrentUserService(farmer.Id), null!);

        await service.UpdateAsync(product.Id, new UpdateProductDto
        {
            Name = "Roșii cherry",
            Description = "Roșii mici",
            Quantity = 12,
            Price = 10m,
            UnitOfMeasurement = "kg",
            CategoryId = newCategory.Id
        });

        var updated = await context.Products.SingleAsync(p => p.Id == product.Id);
        Assert.Equal("Roșii cherry", updated.Name);
        Assert.Equal("Roșii mici", updated.Description);
        Assert.Equal(12, updated.Quantity);
        Assert.Equal(10m, updated.Price);
        Assert.Equal(newCategory.Id, updated.CategoryId);
    }

    [Fact]
    public async Task UpdateAsync_when_product_belongs_to_another_farmer_throws_unauthorized()
    {
        await using var context = TestDbContextFactory.Create();
        var owner = TestData.User("farmer-owner");
        var otherFarmer = TestData.User("farmer-other");
        var category = TestData.Category();
        var product = TestData.Product("Morcovi", 4m, 30, category, owner);

        context.Users.AddRange(owner, otherFarmer);
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context, new FakeCurrentUserService(otherFarmer.Id), null!);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.UpdateAsync(product.Id, new UpdateProductDto
            {
                Name = "Morcovi bio",
                Description = "Altă descriere",
                Quantity = 10,
                Price = 6m,
                UnitOfMeasurement = "kg",
                CategoryId = category.Id
            }));
    }

    [Fact]
    public async Task SetMainImageAsync_sets_selected_image_as_main_and_unsets_existing_main_image()
    {
        await using var context = TestDbContextFactory.Create();
        var farmer = TestData.User("farmer-1");
        var category = TestData.Category();
        var product = TestData.Product("Pere", 7m, 20, category, farmer);
        var firstImage = new ProductImages { ImageUrl = "pere-1.jpg", IsMain = true, Product = product };
        var secondImage = new ProductImages { ImageUrl = "pere-2.jpg", IsMain = false, Product = product };
        product.Images!.Add(firstImage);
        product.Images.Add(secondImage);

        context.Users.Add(farmer);
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context, new FakeCurrentUserService(farmer.Id), null!);

        await service.SetMainImageAsync(secondImage.Id);

        var images = await context.ProductImages.OrderBy(i => i.Id).ToListAsync();
        Assert.False(images[0].IsMain);
        Assert.True(images[1].IsMain);
    }

    [Fact]
    public async Task SetMainImageAsync_when_product_belongs_to_another_farmer_throws_unauthorized()
    {
        await using var context = TestDbContextFactory.Create();
        var owner = TestData.User("farmer-owner");
        var otherFarmer = TestData.User("farmer-other");
        var category = TestData.Category();
        var product = TestData.Product("Prune", 6m, 20, category, owner);
        var image = new ProductImages { ImageUrl = "prune.jpg", Product = product };
        product.Images!.Add(image);

        context.Users.AddRange(owner, otherFarmer);
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context, new FakeCurrentUserService(otherFarmer.Id), null!);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.SetMainImageAsync(image.Id));
    }

    [Fact]
    public async Task GetNearbyProductsAsync_filters_by_distance_and_orders_by_nearest_farmer()
    {
        await using var context = TestDbContextFactory.Create();
        var vegetables = TestData.Category("Legume", "🥕");
        var nearFarmer = TestData.User(
            "farmer-near",
            firstName: "Ion",
            lastName: "Popescu",
            city: "Craiova",
            county: "Dolj",
            latitude: 44.3302,
            longitude: 23.7949);
        var fartherFarmer = TestData.User(
            "farmer-farther",
            firstName: "Maria",
            lastName: "Ionescu",
            city: "București",
            county: "București",
            latitude: 44.4268,
            longitude: 26.1025);

        context.Users.AddRange(nearFarmer, fartherFarmer);
        context.Categories.Add(vegetables);
        context.Products.AddRange(
            TestData.Product("Roșii", 8m, 20, vegetables, nearFarmer),
            TestData.Product("Cartofi", 4m, 20, vegetables, fartherFarmer)
        );
        await context.SaveChangesAsync();

        var service = new ProductService(context, new FakeCurrentUserService("customer-1"), null!);

        var result = await service.GetNearbyProductsAsync(
            userLat: 44.3302,
            userLng: 23.7949,
            maxDistanceKm: 50,
            categoryId: vegetables.Id,
            minPrice: null,
            maxPrice: null);

        var product = Assert.Single(result);
        Assert.Equal("Roșii", product.Name);
        Assert.Equal("Craiova", product.FarmerCity);
        Assert.True(product.DistanceKm <= 1);
    }
}
