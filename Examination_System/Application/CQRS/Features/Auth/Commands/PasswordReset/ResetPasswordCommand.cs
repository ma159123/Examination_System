using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Auth.Commands.PasswordReset;

public record ResetPasswordCommand(string token, string NewPassword) : IRequest<Result>;
