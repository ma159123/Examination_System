
using Application.Interfaces;
using Domain.Entites;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services;

public class TokenGenerator : ITokenGenerator
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _dbContext;
    public TokenGenerator(IConfiguration config, AppDbContext dbContext)
    {
        _config = config;
        _dbContext = dbContext;
    }

    public (string accessToken, string refreshToken) GenerateTokens(AppUser user, IList<string> roles)
    {
        // 1. Generate JWT Access Token (15 Min Expiry)
        var secretKey = _config["JwtSettings:Secret"]
            ?? throw new InvalidOperationException("JWT Secret is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add User Roles to Token Claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15), // 15 Min Expiry
            SigningCredentials = credentials,
            Issuer = _config["JwtSettings:Issuer"],
            Audience = _config["JwtSettings:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        // 2. Generate Cryptographically Secure Refresh Token
        var refreshToken = GenerateRefreshToken();

        return (accessToken, refreshToken);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    // reset token methods
    public async Task<string> GenerateAndSaveResetTokenAsync(string userId, CancellationToken cancellationToken = default)
    {
        // 1. Generate Cryptographically Secure Plain Token (URL-Safe)
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var plainToken = Convert.ToHexString(randomBytes); // 64-char Hex string

        // 2. Hash Token before saving to DB
        var hashedToken = HashToken(plainToken);

        // 3. Save Record with 15-Minute Expiry
        var record = new ResetTokenRecord
        {
            UserId = userId,
            HashedToken = hashedToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15), // 15-min TTL
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.ResetTokenRecords.Add(record);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return plainToken; // Return plain token to be sent in the email link
    }

    public async Task<ResetTokenRecord?> ValidateResetTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var hashedInputToken = HashToken(token);

        var tokenRecord = await _dbContext.ResetTokenRecords
            .Where(t => t.HashedToken == hashedInputToken)
            .FirstOrDefaultAsync(cancellationToken);

        // Check exists, not used, and not expired
        if (tokenRecord == null || tokenRecord.IsUsed || tokenRecord.ExpiresAt < DateTime.UtcNow)
        {
            return null;
        }

        return tokenRecord;
    }

    public async Task InvalidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var hashedInputToken = HashToken(token);

        var tokenRecord = await _dbContext.ResetTokenRecords
            .FirstOrDefaultAsync(t => t.HashedToken == hashedInputToken, cancellationToken);

        if (tokenRecord != null)
        {
            tokenRecord.IsUsed = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}
