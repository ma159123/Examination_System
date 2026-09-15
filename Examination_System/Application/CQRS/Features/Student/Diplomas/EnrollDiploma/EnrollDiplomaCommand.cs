using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Student.Diplomas.EnrollDiploma
{
    public record EnrollDiplomaCommand(Guid DiplomaId) : IRequest<Result>;
}
