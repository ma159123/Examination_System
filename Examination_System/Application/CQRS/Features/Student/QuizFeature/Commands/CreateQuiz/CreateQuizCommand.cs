using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Student.QuizFeature.Commands.CreateQuiz
{
    public record CreateQuizCommand(
      Guid diplomaId,
      string title,
      int durationMinutes,
      int passScore,
      int? maxAttempts
        ) : IRequest<Result<QuizResponseDTO>>;
}
