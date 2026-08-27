using FluentValidation;

namespace Application.CQRS.Features.UserFeature.Commands.IsEmailExist
{
    internal class IsEmailExistQueryValidator : AbstractValidator<IsEmailExistQuery>
    {
        public IsEmailExistQueryValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }

    }
}

