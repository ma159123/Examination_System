using FluentValidation;

namespace Application.CQRS.Features.Auth.Commands.ResendOTP
{
    public class ResendOTPCommandValidator : AbstractValidator<ResendOTPCommand>
    {
        public ResendOTPCommandValidator()
        {

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.");

        }
    }
}