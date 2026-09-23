using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Student.QuizFeature.AnswerQuestion
{
    public record AnswerQuestionRequest(Guid QuestionId, Guid SelectedOptionId);

    public record AnswerQuestionCommand(
        Guid AttemptId,
        Guid QuestionId,
        Guid SelectedOptionId
    ) : IRequest<Result<AnswerQuestionResponse>>;

    public record AnswerQuestionResponse(bool Saved = true);
}
