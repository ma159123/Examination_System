using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Otp.GenerateOtp;

public record GenerateAndSendOtpCommand(
string UserId,
string Email
) : IRequest<Result>;
