using System.Security.Claims;
using System.Text;
using JobTracker.Application.Features.Auth.Common;
using JobTracker.Domain.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace JobTracker.Infrastructure.Authentication;

public sealed class JwtTokenService(IConfiguration configuration, TimeProvider timeProvider) : IJwtTokenService
{
    public string CreateToken(User user)
    {
        var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var issuer = configuration["Jwt:Issuer"] ?? "JobTracker";
        var audience = configuration["Jwt:Audience"] ?? "JobTracker";
        var expiryMinutes = int.TryParse(configuration["Jwt:ExpiryMinutes"], out var configuredExpiryMinutes)
            ? configuredExpiryMinutes
            : 60;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName)
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = timeProvider.GetUtcNow().AddMinutes(expiryMinutes).UtcDateTime,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256)
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}

