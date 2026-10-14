using Application.CQRS.Features.Student.Diplomas.Commands.CreateDiploma;
using Application.CQRS.Features.Student.Diplomas.Commands.EnrollDiploma;
using Application.CQRS.Features.Student.Diplomas.Queries.GetAllDiplomas;
using Application.CQRS.Features.Student.Diplomas.Queries.GetDiplomaById;
using Application.CQRS.Features.Student.QuizFeature.Queries.GetDiplomaQuizes;
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
        //-------------------admin -------------------//
        [HttpPost("admin/diplomas")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDiploma([FromBody] CreateDiplomaCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                if (result.Error.Code == "ValidationError" || result.Error.Code == "422")
                {
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, new
                    {
                        title = "Validation Failed",
                        status = 422,
                        errors = result.Error.Description
                    });
                }

                return BadRequest(result.Error);
            }

            return StatusCode(StatusCodes.Status201Created, result.Value);
        }

        [HttpPut("admin/diplomas")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDiploma([FromBody] CreateDiplomaCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                if (result.Error.Code == "ValidationError" || result.Error.Code == "422")
                {
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, new
                    {
                        title = "Validation Failed",
                        status = 422,
                        errors = result.Error.Description
                    });
                }

                return BadRequest(result.Error);
            }

            return StatusCode(StatusCodes.Status201Created, result.Value);
        }

    }
}