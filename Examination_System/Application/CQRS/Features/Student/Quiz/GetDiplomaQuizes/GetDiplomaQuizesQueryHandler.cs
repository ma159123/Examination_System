using Application.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Features.Student.Quiz.GetDiplomaQuizes
{
    public class GetDiplomaQuizesQueryHandler : IRequestHandler<GetDiplomaQuizesQuery, Result<List<GetQuizResponse>>>
    {
        private readonly IDiplomaRepo _diplomaRepo;
        private readonly ICurrentUserService _currentUserService;
        public GetDiplomaQuizesQueryHandler(IDiplomaRepo diplomaRepo, ICurrentUserService currentUserService)
        {
            _diplomaRepo = diplomaRepo;
            _currentUserService = currentUserService;
        }
        public async Task<Result<List<GetQuizResponse>>> Handle(GetDiplomaQuizesQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return Result.Failure<List<GetQuizResponse>>(Error.Unauthorized, "User not authenticated.");
            }

            var isDiplomaExists = await _diplomaRepo.IsDiplomaExistsAsync(request.DiplomaId, cancellationToken);
            if (!isDiplomaExists)
            {
                return Result.Failure<List<GetQuizResponse>>(Error.NotFound, "Diploma not found.");
            }

            // 2. is enrolled in diploma
            var isEnrolled = await _diplomaRepo.IsDiplomaEnrolledAsync(request.DiplomaId, (Guid)userId, cancellationToken);

            if (!isEnrolled)
            {
                return Result.Failure<List<GetQuizResponse>>(Error.Forbidden, "You must be enrolled in this diploma to access its quizzes.");
            }

            var diplomaQuizes = await _diplomaRepo.GetDiplomasQuizes(request.DiplomaId, (Guid)userId).ToListAsync();


            return Result.Success(diplomaQuizes);
        }
    }
}
