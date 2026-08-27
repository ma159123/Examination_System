using FluentValidation;

namespace Application.CQRS.Features.Auth.Commands.PasswordReset
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {

            RuleFor(x => x.token)
                .NotEmpty()
                .WithMessage("Token is required.");

            RuleFor(x => x.NewPassword)
 .NotEmpty().WithMessage("New password is required.")
 .MinimumLength(8).WithMessage(" password must be at least 8 characters long.")
 .Matches("[A-Z]").WithMessage(" password must contain at least one uppercase letter.")
 .Matches("[a-z]").WithMessage(" password must contain at least one lowercase letter.")
 .Matches("[0-9]").WithMessage(" password must contain at least one digit.")
 .Matches("[^a-zA-Z0-9]").WithMessage(" password must contain at least one special character.");

        }
    }
}
