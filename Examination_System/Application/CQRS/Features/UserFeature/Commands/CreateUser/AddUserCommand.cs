using Domain.Common;
using Domain.Entites;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Features.UserFeature.Commands.CreateUser
{
    public record AddUserCommand(string Email, string Password, string FullName, UserRoles Role) : IRequest<Result<AppUser>>;
}
