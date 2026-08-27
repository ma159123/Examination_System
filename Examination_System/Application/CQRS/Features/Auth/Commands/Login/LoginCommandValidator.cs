using FluentValidation;

namespace Application.CQRS.Features.Auth.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<UserLoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                            .NotEmpty().WithMessage("Email is required.")
                            .EmailAddress().WithMessage("A valid email address is required.");


            RuleFor(x => x.Password)
  .NotEmpty().WithMessage("Password is required.")
  .MinimumLength(8).WithMessage("Incorrect or Invalid Password")
  .Matches("[A-Z]").WithMessage("Incorrect or Invalid Password.")
  .Matches("[a-z]").WithMessage("Incorrect or Invalid Password.")
  .Matches("[0-9]").WithMessage("Incorrect or Invalid Password.")
  .Matches("[^a-zA-Z0-9]").WithMessage("Incorrect or Invalid Password.");

        }
    }
}
