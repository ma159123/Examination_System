using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entites;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
namespace Infrastructure.Services;

public class OtpService : IOtpService
{
    private readonly AppDbContext _dbContext;

    public OtpService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> VerifyOTP(string UserId, string Otp, CancellationToken ct)
    {
        var otpRecord = await _dbContext.Set<OtpRecord>()
             .Where(o => o.UserId == UserId && !o.IsUsed).FirstOrDefaultAsync(ct);

        if (otpRecord == null || otpRecord.ExpiresAt < DateTime.UtcNow)
            return Result.Failure(new Error("Auth.OtpExpired", "OTP code has expired or is invalid.", 400)); ;

        bool isValid = BCrypt.Net.BCrypt.Verify(Otp, otpRecord.HashedOtp);
        if (!isValid)
            return Result.Failure(new Error("Auth.InvalidOtp", "Invalid OTP code provided.", 400));

        // Mark as used
        otpRecord.IsUsed = true;
        await _dbContext.SaveChangesAsync(ct);

        return Result.Success(message: "Otp verified successfully.");
    }

    public async Task<Result<string>> GenerateAndStoreOtpAsync(string userId, CancellationToken cancellationToken)
    {
        var oneHourAgo = DateTime.UtcNow.AddHours(-1);
        // 1. Check Rate Limit (Max 3 resends per hour)
        var recentOtpsCount = await _dbContext.OtpRecords
            .CountAsync(o => o.UserId == userId && o.CreatedAt >= oneHourAgo, cancellationToken);

        if (recentOtpsCount >= 3)
        {
            return Result.Failure<string>(Error.ResendLimitExceeded);
        }
        // 1. Generate Cryptographically Secure 6-Digit OTP
        var otpCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        // 2. Hash OTP code before saving to DB
        var hashedOtp = BCrypt.Net.BCrypt.HashPassword(otpCode, workFactor: 10);

        // 3. Create Record with 10-minute TTL
        var otpRecord = new OtpRecord
        {
            UserId = userId,
            HashedOtp = hashedOtp,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            CreatedAt = DateTime.UtcNow,
            IsUsed = false
        };
        // 4. Invalidate any previous unused OTPs for the user
        await _dbContext.OtpRecords
            .Where(o => o.UserId == userId && !o.IsUsed)
            .ExecuteUpdateAsync(s => s.SetProperty(o => o.IsUsed, true), cancellationToken);
        _dbContext.OtpRecords.Add(otpRecord);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Return plain OTP code to be sent via email
        return Result.Success<string>(data: otpCode);
    }
}