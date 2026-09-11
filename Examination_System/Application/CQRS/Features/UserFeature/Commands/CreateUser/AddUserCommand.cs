using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.UserFeature.Commands.CreateUser
{
    public record AddUserCommand(string Email, string Password, string FullName, string Role) : IRequest<Result<string>>;
}
