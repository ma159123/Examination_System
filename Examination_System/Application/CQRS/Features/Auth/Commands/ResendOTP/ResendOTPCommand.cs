using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Auth.Commands.ResendOTP
{
    public record ResendOTPCommand(string Email) : IRequest<Result>;
}
