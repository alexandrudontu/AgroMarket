using System.Security.Claims;

namespace Backend.Services.Interfaces
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        string Email { get; }
    }
}