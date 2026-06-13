using Backend.Models;

namespace Backend.Tests.Helpers;

internal static class TestData
{
    public static User User(
        string id,
        string firstName = "Test",
        string lastName = "User",
        string? city = null,
        string? county = null,
        double? latitude = null,
        double? longitude = null)
    {
        return new User
        {
            Id = id,
            UserName = $"{id}@example.com",
            Email = $"{id}@example.com",
            FirstName = firstName,
            LastName = lastName,
            City = city,
            County = county,
            Latitude = latitude,
            Longitude = longitude
        };
    }

    public static Category Category(string name = "Legume", string icon = "🥕")
    {
        return new Category
        {
            Name = name,
            Icon = icon,
            Products = new List<Product>()
        };
    }

    public static Product Product(
        string name,
        decimal price,
        int quantity,
        Category category,
        User farmer,
        string unitOfMeasurement = "kg")
    {
        return new Product
        {
            Name = name,
            Description = $"Descriere {name}",
            Price = price,
            Quantity = quantity,
            UnitOfMeasurement = unitOfMeasurement,
            Category = category,
            Farmer = farmer,
            FarmerId = farmer.Id,
            Images = new List<ProductImages>()
        };
    }
}
