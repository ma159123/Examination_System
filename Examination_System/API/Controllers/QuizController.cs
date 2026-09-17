using Application.CQRS.Features.Student.QuizFeature.QuizAttemptHistory;
using Application.CQRS.Features.Student.QuizFeature.StartQuiz;
using Domain.Common.Errors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IMediator _mediator;
        public QuizController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("student/attempts")]
        [Authorize]
        public async Task<IActionResult> GetStudentAttempts(Guid? quizId,
     Guid? diplomaId,
     int page = 1,
     int perPage = 20)
        {
            var result = await _mediator.Send(new GetStudentAttemptsQuery(quizId, diplomaId, page, perPage));

            return Ok(result);
        }

        [HttpPost("quizzes/{quizId}/start")]
        [Authorize]
        public async Task<IActionResult> StartQuiz(Guid quizId)
        {
            var result = await _mediator.Send(new StartQuizCommand(quizId));

            if (!result.IsSuccess)
            {
                if (result.Error.Code == "409")
                    return Conflict(new { message = result.Error.Description });

                if (result.Error.Code == Error.NotFound.Code)
                    return NotFound(result.Error);

                if (result.Error.Code == Error.Forbidden.Code)
                    return StatusCode(StatusCodes.Status403Forbidden, result.Error);

                return BadRequest(result.Error);
            }

            return StatusCode(StatusCodes.Status201Created, result.Value);
        }
    }
}
