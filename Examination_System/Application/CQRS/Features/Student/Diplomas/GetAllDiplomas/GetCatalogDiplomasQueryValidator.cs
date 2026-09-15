using FluentValidation;

namespace Application.CQRS.Features.Student.Diplomas.GetAllDiplomas
{
    public class GetCatalogDiplomasQueryValidator : AbstractValidator<GetCatalogDiplomasQuery>
    {
        public GetCatalogDiplomasQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.");
            RuleFor(x => x.PerPage)
                .GreaterThan(0).WithMessage("Items per page must be greater than 0.");
        }
    }
}