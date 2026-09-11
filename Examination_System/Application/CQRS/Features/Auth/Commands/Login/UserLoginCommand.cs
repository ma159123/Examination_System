using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Auth.Commands.Login
{
    public record LoginResponse(string userId, string? role, string AccessToken, DateTime accessTokenExpiry, string RefreshToken, DateTime refreshTokenExpiry);
    public record UserLoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;
}
