namespace Backend.DTOs.Categories
{
    public class CategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Icon { get; set; } = "🛒";

        public int ProductsCount { get; set; }
    }
}
