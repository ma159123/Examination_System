using Application.Interfaces;
using Application.Interfaces.Admin;
using Domain.Common;
using Domain.Entites;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Features.Admin.AdminDiploma.Commands.CreateDiploma
{
    public class CreateDiplomaCommnadHandler : IRequestHandler<CreateDiplomaCommand, Result<CreateDiplomaResponse>>
    {
        private readonly IAdminDiplomaRepo _adminDiplomaRepo;
        private readonly ICurrentUserService _currentUserService;
        public CreateDiplomaCommnadHandler(IAdminDiplomaRepo adminDiplomaRepo, ICurrentUserService currentUserService)
        {
            _adminDiplomaRepo = adminDiplomaRepo;
            _currentUserService = currentUserService;
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

            await _adminDiplomaRepo.CreateDiplomaAsync(diploma, cancellationToken);


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
