using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entites;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.CQRS.Features.Auth.Commands.Login
{
    public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, Result<LoginResponse>>
    {
        private readonly IMediator _mediator;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenGenerator _tokenGenerator;
        public UserLoginCommandHandler(IMediator mediator, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenGenerator tokenGenerator)
        {
            _mediator = mediator;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
        }
        public async Task<Result<LoginResponse>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            // 1. Find user by Email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Do not reveal that the email doesn't exist
                return Result.Failure<LoginResponse>(new Error("Auth.InvalidCredentials", "Invalid email or password.", 401));
            }

            // 2. Check Account Verification Status (403 Forbidden)
            if (user.Status != UserStatus.Active || !user.EmailConfirmed)
            {
                return Result.Failure<LoginResponse>(new Error("Auth.AccountNotVerified", "Account not verified", 403));
            }

            // 3. Validate Password with Lockout Enabled (5 consecutive attempts -> 15 min lock)
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                return Result.Failure<LoginResponse>(new Error("Auth.AccountLocked", "Account locked due to multiple failed attempts. Try again later.", 429));
            }

            if (!result.Succeeded)
            {
                return Result.Failure<LoginResponse>(new Error("Auth.InvalidCredentials", "Invalid email or password.", 401));
            }

            // 4. Generate Tokens
            var roles = await _userManager.GetRolesAsync(user);
            var (accessToken, accessTokenExpiry, refreshToken, refreshTokenExpiry) = await _tokenGenerator.GenerateAndSaveTokensAsync(user, roles);

            // TODO: Save RefreshToken in DB/Redis for rotation support

            var response = new LoginResponse(user.Id, roles.FirstOrDefault(), accessToken, accessTokenExpiry, refreshToken, refreshTokenExpiry);
            return Result.Success(response);
        }
    }
}
