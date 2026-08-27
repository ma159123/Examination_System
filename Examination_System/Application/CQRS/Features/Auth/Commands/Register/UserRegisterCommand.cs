using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Auth.Commands.Register
{
    public record RegisterResponse(string UserId, string Email, string Message);
    public record UserRegisterCommand(string Email, string Password, string FullName) : IRequest<Result<RegisterResponse>>;
}
