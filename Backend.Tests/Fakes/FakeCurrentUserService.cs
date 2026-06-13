using Backend.Services.Interfaces;

namespace Backend.Tests.Fakes;

internal sealed class FakeCurrentUserService : ICurrentUserService
{
    public FakeCurrentUserService(string userId, string email = "test@example.com")
    {
        UserId = userId;
        Email = email;
    }

    public string UserId { get; }

    public string Email { get; }
}
