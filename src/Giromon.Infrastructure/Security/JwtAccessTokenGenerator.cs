using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Giromon.Application.Abstractions.Security;
using Giromon.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Giromon.Infrastructure.Security;

public sealed class JwtAccessTokenGenerator
    : IAccessTokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtAccessTokenGenerator(JwtSettings settings)
    {
        _settings = settings;
    }

    public string Generate(User user)
    {
        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new Claim(
                JwtRegisteredClaimNames.Email,
                user.Email),

            new Claim(
                JwtRegisteredClaimNames.Name,
                user.Name),

            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.Secret));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                _settings.ExpirationInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}