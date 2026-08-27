using FluentValidation;

namespace Application.CQRS.Features.Auth.Commands.ForgotPassword
{
    internal class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordValidator()
        {

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

        }
    }
}
