using Backend.DTOs.Orders;
using Backend.DTOs.Products;

namespace Backend.DTOs.Farmer
{
    public class FarmerDetailsDto
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int ProductsCount { get; set; }

        public int OrdersCount { get; set; }

        public List<ProductListDto> Products { get; set; }

        public List<OrderResponseDto> Orders { get; set; }
    }
}
