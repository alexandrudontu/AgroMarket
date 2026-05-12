using Backend.DTOs.Images;
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Products
{
    public class ProductListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        public string UnitOfMeasurement { get; set; }  
        public string CategoryName { get; set; }
        [Range(0, int.MaxValue)]
        public decimal Quantity { get; set; }
        public int CategoryId { get; set; }
        public string FarmerName { get; set; }
        public List<ProductImageDto> ?ProductImages { get; set; }
    }
}
