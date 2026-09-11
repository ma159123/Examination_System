
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

    public async Task<(string accessToken, DateTime accessTokenExpiry, string refreshToken, DateTime refreshTokenExpiry)>
          GenerateAndSaveTokensAsync(AppUser user, IList<string> roles, CancellationToken cancellationToken = default)
    {
        // 1. Generate Access Token
        var secretKey = _config["JwtSettings:Secret"]
            ?? throw new InvalidOperationException("JWT Secret is not configured.");

        var accessTokenMinutes = double.TryParse(_config["JwtSettings:AccessTokenExpirationMinutes"], out var accMin) ? accMin : 15;
        var refreshTokenDays = double.TryParse(_config["JwtSettings:RefreshTokenExpirationDays"], out var refDays) ? refDays : 7;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var accessTokenExpiry = DateTime.UtcNow.AddMinutes(accessTokenMinutes);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = accessTokenExpiry,
            SigningCredentials = credentials,
            Issuer = _config["JwtSettings:Issuer"],
            Audience = _config["JwtSettings:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        // 2. Generate Refresh Token
        var (plainRefreshToken, refreshTokenExpiry) = GenerateRawRefreshToken(refreshTokenDays);

        // 3. Save Hashed Refresh Token in DB
        var hashedRefreshToken = HashToken(plainRefreshToken);

        var refreshTokenRecord = new RefreshTokenRecord
        {
            UserId = user.Id,
            HashedToken = hashedRefreshToken,
            ExpiresAt = refreshTokenExpiry,
            IsRevoked = false,
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.RefreshTokenRecords.Add(refreshTokenRecord);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Return plain token to client, saved hashed version in DB
        return (accessToken, accessTokenExpiry, plainRefreshToken, refreshTokenExpiry);
    }

    public async Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        var hashedToken = HashToken(refreshToken);

        var tokenRecord = await _dbContext.RefreshTokenRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.UserId == userId && t.HashedToken == hashedToken, cancellationToken);

        if (tokenRecord == null) return false;
        if (tokenRecord.IsRevoked || tokenRecord.IsUsed) return false;
        if (tokenRecord.ExpiresAt < DateTime.UtcNow) return false;

        return true;
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var hashedToken = HashToken(refreshToken);

        var tokenRecord = await _dbContext.RefreshTokenRecords
            .FirstOrDefaultAsync(t => t.HashedToken == hashedToken, cancellationToken);

        if (tokenRecord != null)
        {
            tokenRecord.IsRevoked = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static (string refreshToken, DateTime expiry) GenerateRawRefreshToken(double daysToExpire)
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return (Convert.ToBase64String(randomNumber), DateTime.UtcNow.AddDays(daysToExpire));
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
