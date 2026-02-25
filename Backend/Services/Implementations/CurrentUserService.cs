using System.Security.Claims;
using Backend.Services.Interfaces;

namespace Backend.Services.Implementations
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId =>
            int.Parse(_httpContextAccessor.HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier));

        public string Email =>
            _httpContextAccessor.HttpContext.User
                .FindFirstValue(ClaimTypes.Email);
    }
}
