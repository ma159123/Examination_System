using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Auth.Commands.Register
{
    public record UserRegisterCommand(string Email, string Password, string FullName) : IRequest<Result<UserRegistrationResponseDto>>;
}
