using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Student.QuizFeature.GetDiplomaQuizes
{
    public record GetDiplomaQuizesQuery(Guid DiplomaId) : IRequest<Result<List<GetQuizResponse>>>;

}
