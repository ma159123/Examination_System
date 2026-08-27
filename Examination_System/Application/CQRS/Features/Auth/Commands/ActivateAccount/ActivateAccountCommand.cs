
using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Auth.Commands.ActivateAccount;

public record ActivateAccountCommand(string Email, string Otp) : IRequest<Result>;