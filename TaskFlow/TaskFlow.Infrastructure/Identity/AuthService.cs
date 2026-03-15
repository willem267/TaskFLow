using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs.Auth;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence.Context;

namespace TaskFlow.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }
    
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
       var exists = await _context.Users.AnyAsync(u=>u.Email == request.Email);
       
       if(exists) throw new InvalidOperationException("Email already in use.");

       var user = new User
       {
           Id = Guid.NewGuid(),
           Email = request.Email,
           Name = request.Name,
           PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
           CreatedAt = DateTime.UtcNow,
       };
       
       await  _context.Users.AddAsync(user);
       await _context.SaveChangesAsync();

       return await GenerateAuthResponseAsync(user);

    }
    

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
       var user = await _context.Users.FirstOrDefaultAsync(u=>u.Email==request.Email.ToLower().Trim());

       if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
           throw new UnauthorizedAccessException("Invalid email or password");
       
       return await GenerateAuthResponseAsync(user);

    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .Include(t=>t.User)
            .FirstOrDefaultAsync(t => t.Token == refreshToken);
        
        if(token is null || token.ExpiresAt < DateTime.UtcNow || token.IsRevoked==true)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        token.IsRevoked = true;
        
        var response = await GenerateAuthResponseAsync(token.User);
        await _context.SaveChangesAsync();

        return response;

    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);

        if (token is null)
            return;
        
        token.IsRevoked = true;
        await _context.SaveChangesAsync();
    }
    
    //Private helpers
    private async Task<AuthResponse> GenerateAuthResponseAsync(User user)
    {
        var acessToken = GenerateAccesToken(user);
        var refreshToken = await CreateRefreshTokenAsync(user.Id);
        var expiresAt = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:AccessTokenExpirationMinutes"));

        return new AuthResponse(
            acessToken, refreshToken.Token, expiresAt, new UserDto(user.Id, user.Email, user.Name, user.Avatar));
        
    }

    private string GenerateAccesToken(User user)
    {
        var secret= _config["Jwt:Secret"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                _config.GetValue<int>("Jwt:AccessTokenExpirationMinutes")),
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);

    }

    private async Task<RefreshToken> CreateRefreshTokenAsync(Guid userId)
    {
        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresAt = DateTime.UtcNow.AddDays(_config.GetValue<int>("Jwt:RefreshTokenExpirationDays")),
            CreatedAt = DateTime.UtcNow
        };
        
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();

        return token;
    }
    
    
}