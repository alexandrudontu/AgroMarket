using Backend.Data;
using Backend.DTOs.Cart;
using Backend.Models;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public CartService(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<CartDto> GetCartAsync()
        {
            var cart = await GetOrCreateCart();

            var items = cart.CartItems.Select(i => new CartItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                UnitPrice = i.Product.Price,
                LineTotal = i.Quantity * i.Product.Price
            }).ToList();

            return new CartDto
            {
                Items = items,
                TotalAmount = items.Sum(i => i.LineTotal)
            };
        }

        public async Task AddToCartAsync(AddToCartDto dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);

            if (product == null)
                throw new Exception("Product not found");


            var cart = await GetOrCreateCart();

            var item = cart.CartItems
                .FirstOrDefault(i => i.ProductId == dto.ProductId);

            if (item != null)
            {
                item.Quantity += dto.Quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int productId)
        {
            var cart = await GetOrCreateCart();

            var item = cart.CartItems
                .FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync()
        {
            var cart = await GetOrCreateCart();

            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();
        }

        private async Task<Cart> GetOrCreateCart()
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == _currentUser.UserId);

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = _currentUser.UserId,
                    CartItems = new List<CartItem>()
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }
    }
}
