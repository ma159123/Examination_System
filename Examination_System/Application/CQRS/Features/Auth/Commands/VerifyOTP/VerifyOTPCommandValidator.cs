using FluentValidation;

namespace Application.CQRS.Features.Auth.Commands.VerifyOTP
{
    public class VerifyOTPCommandValidator : AbstractValidator<VerifyOTPCommand>
    {
        public VerifyOTPCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Otp).NotEmpty().Length(6);
        }
    }
}
