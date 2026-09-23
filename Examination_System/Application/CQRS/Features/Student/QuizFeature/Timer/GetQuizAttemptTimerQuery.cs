using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Student.QuizFeature.Timer
{
    public record GetQuizAttemptTimerQuery(Guid AttemptId) : IRequest<Result<int>>;


}
