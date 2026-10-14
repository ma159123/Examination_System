using Application.Interfaces;
using Domain.Common;
using Domain.Entites;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Features.Student.Diplomas.Commands.CreateDiploma
{
    public class CreateDiplomaCommnadHandler : IRequestHandler<CreateDiplomaCommand, Result<CreateDiplomaResponse>>
    {
        private readonly IDiplomaRepo _diplomaRepo;
        public CreateDiplomaCommnadHandler(IDiplomaRepo diplomaRepo, ICurrentUserService currentUserService)
        {
            _diplomaRepo = diplomaRepo;
        }
        public async Task<Result<CreateDiplomaResponse>> Handle(CreateDiplomaCommand request, CancellationToken cancellationToken)
        {

            var diploma = new Diploma
            {
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                Status = ContentStatus.Draft,
                CreatedAt = DateTime.UtcNow
            };

            await _diplomaRepo.CreateDiplomaAsync(diploma, cancellationToken);


            return Result.Success(new CreateDiplomaResponse(
                diploma.Id,
                diploma.Title,
                diploma.Description,
                diploma.Status.ToString(),
                diploma.CreatedAt
            ));
        }
    }
}
