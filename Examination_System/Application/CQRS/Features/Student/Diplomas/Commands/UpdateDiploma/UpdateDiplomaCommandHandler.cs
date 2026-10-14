using Application.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using MediatR;

namespace Application.CQRS.Features.Student.Diplomas.Commands.UpdateDiploma
{
    public class UpdateDiplomaCommandHandler : IRequestHandler<UpdateDiplomaCommand, Result<GetDiplomaResponse>>
    {
        private readonly IDiplomaRepo _diplomaRepo;
        public async Task<Result<GetDiplomaResponse>> Handle(UpdateDiplomaCommand request, CancellationToken cancellationToken)
        {
            var diploma = _diplomaRepo.GetDiplomaById(request.diplomaId);
            if (diploma == null)
            {
                return Result.Failure<GetDiplomaResponse>(new Error("DiplomaNotFound", $"Diploma with ID {request.diplomaId} not found."));
            }
            diploma.Title = request.Title;
            diploma.Description = request.Description;

            await _diplomaRepo.UpdateDiplomaAsync(diploma, cancellationToken);


            return Result.Success(new GetDiplomaResponse
            {
                Id = diploma.Id,
                Title = diploma.Title,
                Description = diploma.Description,
                Status = diploma.Status,
            });
        }
    }
}
