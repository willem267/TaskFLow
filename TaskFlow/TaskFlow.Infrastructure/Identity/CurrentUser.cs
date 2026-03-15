using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Infrastructure.Identity;

public class CurrentUser : ICurrentUser
{
    public Guid Id { get; }
    public string Email { get; }
    public string Name { get; }

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;

        // ASP.NET Core map "sub" → ClaimTypes.NameIdentifier
        var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier)
                      ?? user?.FindFirst(JwtRegisteredClaimNames.Sub);

        var emailClaim = user?.FindFirst(ClaimTypes.Email)
                         ?? user?.FindFirst(JwtRegisteredClaimNames.Email);

        var nameClaim = user?.FindFirst("name");

        if (idClaim is null)
            throw new UnauthorizedAccessException("Invalid token.");

        Id = Guid.Parse(idClaim.Value);
        Email = emailClaim?.Value ?? string.Empty;
        Name = nameClaim?.Value ?? string.Empty;
    }
}