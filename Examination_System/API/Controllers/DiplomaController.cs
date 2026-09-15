using Application.CQRS.Features.Student.Diplomas.EnrollDiploma;
using Application.CQRS.Features.Student.Diplomas.GetAllDiplomas;
using Application.CQRS.Features.Student.Diplomas.GetDiplomaById;
using Application.CQRS.Features.Student.Quiz.GetDiplomaQuizes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiplomaController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DiplomaController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("diplomas")]
        [Authorize]
        public async Task<IActionResult> GetAllCatalogDiplomas(GetCatalogDiplomasQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        [HttpGet("diplomas/{diplomaId}")]
        [Authorize]
        public async Task<IActionResult> GetDiplomaById(Guid diplomaId)
        {
            var result = await _mediator.Send(new GetDiplomaByIdQuery(diplomaId));

            return Ok(result);
        }

        [HttpGet("diplomas/{diplomaId}/quizzes")]
        [Authorize]
        public async Task<IActionResult> GetDiplomaQuizzes(Guid diplomaId)
        {
            var result = await _mediator.Send(new GetDiplomaQuizesQuery(diplomaId));

            return Ok(result);
        }

        [HttpPost("diplomas/enroll")]
        [Authorize]
        public async Task<IActionResult> EnrollDiploma(EnrollDiplomaCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

    }
}