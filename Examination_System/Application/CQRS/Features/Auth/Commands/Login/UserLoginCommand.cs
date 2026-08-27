using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Auth.Commands.Login
{
    public record LoginResponse(string AccessToken, string RefreshToken);
    public record UserLoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;
}
