using FluentValidation;

namespace Application.CQRS.Features.UserFeature.Commands.IsEmailExist
{
    internal class IsUserExistQueryValidator : AbstractValidator<IsUserExistQuery>
    {
        public IsUserExistQueryValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }

    }
}

