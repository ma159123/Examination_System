using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.CQRS.Features.Student.QuizFeature.Commands.StartQuiz
{
    public record StartQuizCommand(Guid QuizId) : IRequest<Result<StartQuizResponse>>;

}
