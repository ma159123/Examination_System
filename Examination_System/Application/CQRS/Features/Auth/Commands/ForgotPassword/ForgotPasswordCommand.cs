using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;
