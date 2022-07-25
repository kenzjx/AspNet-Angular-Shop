using System.Security.Claims;
using Application.Abtractions;
using Microsoft.AspNetCore.Http;

namespace ClassLibrary1.Services;

public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        Guid.TryParse(httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);

        UserId = userId;
        IsAuthenticated = UserId != default;
    }
    
    public Guid UserId { get; }
    public bool IsAuthenticated { get; }
}