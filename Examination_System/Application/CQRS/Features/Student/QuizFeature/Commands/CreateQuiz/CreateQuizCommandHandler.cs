using Application.CQRS.Mapping.QuizMapping;
using Application.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entites;
using MediatR;

namespace Application.CQRS.Features.Student.QuizFeature.Commands.CreateQuiz
{
    public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand, Result<QuizResponseDTO>>
    {
        private readonly IQuizRepo _quizRepo;
        private readonly IDiplomaRepo _diplomaRepo;
        public CreateQuizCommandHandler(IQuizRepo quizRepo, IDiplomaRepo diplomaRepo)
        {
            _quizRepo = quizRepo;
            _diplomaRepo = diplomaRepo;
        }
        public async Task<Result<QuizResponseDTO>> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
        {
            // 1.check if the diploma exists
            var diplomaExists = await _diplomaRepo.IsDiplomaExistsAsync(request.diplomaId, cancellationToken);
            if (!diplomaExists)
            {
                return Result.Failure<QuizResponseDTO>(Error.NotFound, "Diploma does not exist.");
            }

            // 2. create Domain Entity
            var quiz = new Quiz
            {
                DiplomaId = request.diplomaId,
                Title = request.title,
                DurationMinutes = request.durationMinutes,
                PassScore = request.passScore,
                MaxAttempts = request.maxAttempts
            };

            // 3. add the new quiz to the repository and save changes
            await _quizRepo.AddAsync(quiz, cancellationToken);
            await _quizRepo.SaveChangesAsync(cancellationToken);

            // 4. convert the entity to DTO and return the result
            var responseDto = quiz.ToQuizResponseDTO();

            return Result.Success(responseDto);
        }
    }
}
