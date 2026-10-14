using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using MediatR;

namespace Application.CQRS.Features.Student.Diplomas.Commands.DeleteDiploma
{
    public class DeleteDiplomaCommandHandler : IRequestHandler<DeleteDiplomaCommand, Result>
    {
        private readonly IDiplomaRepo _diplomaRepo;
        public DeleteDiplomaCommandHandler(IDiplomaRepo diplomaRepo)
        {
            _diplomaRepo = diplomaRepo;
        }
        public Task<Result> Handle(DeleteDiplomaCommand request, CancellationToken cancellationToken)
        {
            var diploma = _diplomaRepo.GetDiplomaById(request.DiplomaId);
            if (diploma == null)
            {
                return Task.FromResult(Result.Failure(Error.NotFound, "Diploma not found."));
            }
            var isEnrolled = _diplomaRepo.IsDiplomaEnrolledAsync(request.DiplomaId).Result;
            if (isEnrolled)
            {
                return Task.FromResult(Result.Failure(Error.Conflict, "Cannot delete diploma while enrolled."));
            }

            if (diploma.DeletedAt != null)
            {
                return Task.FromResult(Result.Failure(Error.Conflict, "Diploma already deleted."));
            }
            diploma.DeletedAt = DateTime.UtcNow;

            _diplomaRepo.UpdateDiplomaAsync(diploma, cancellationToken);
            return Task.FromResult(Result.Success());
        }
    }
}
