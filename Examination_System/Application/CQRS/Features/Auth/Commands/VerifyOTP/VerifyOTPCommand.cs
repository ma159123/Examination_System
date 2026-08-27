using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Auth.Commands.VerifyOTP;

public record VerifyOTPCommand(string Email, string Otp) : IRequest<Result>;


