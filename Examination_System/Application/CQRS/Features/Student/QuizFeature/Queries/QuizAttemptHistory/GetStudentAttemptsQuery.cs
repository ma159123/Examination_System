using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Student.QuizFeature.Queries.QuizAttemptHistory
{
    public record GetStudentAttemptsQuery(Guid? QuizId, Guid? DiplomaId, int Page = 1, int PerPage = 20) : IRequest<Result<PaginatedListResponse<GetStudentAttemptResponse>>>;
}
