using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using GigBartending.Api.Models;

namespace GigBartending.Api.Services;

public class TokenService
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiryMinutes;

    public TokenService(IConfiguration configuration)
    {
        // Dev-only fallbacks so `dotnet run` works without exporting env vars manually.
        // Never rely on these defaults outside local development.
        _secret = Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? "dev-only-secret-change-in-production-use-minimum-64-characters-for-better-security";
        _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "GigBartendingApp";
        _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "GigBartendingApp";
        _expiryMinutes = int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES"), out var minutes)
            ? minutes
            : 60;
    }

    public string GenerateToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Role, user.Role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (!string.IsNullOrEmpty(user.FirstName)) claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
        if (!string.IsNullOrEmpty(user.LastName)) claims.Add(new Claim(ClaimTypes.Surname, user.LastName));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string Secret => _secret;
    public string Issuer => _issuer;
    public string Audience => _audience;
}
