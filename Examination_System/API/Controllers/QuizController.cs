using Application.CQRS.Features.Student.QuizFeature.AnswerQuestion;
using Application.CQRS.Features.Student.QuizFeature.AttemptResult;
using Application.CQRS.Features.Student.QuizFeature.QuizAttemptHistory;
using Application.CQRS.Features.Student.QuizFeature.StartQuiz;
using Application.CQRS.Features.Student.QuizFeature.SubmitQuiz;
using Application.CQRS.Features.Student.QuizFeature.Timer;
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

        [HttpPost("api/attempts/{attemptId}/answer")]
        [Authorize]
        public async Task<IActionResult> AnswerQuestion(
    Guid attemptId,
    [FromBody] AnswerQuestionRequest request)
        {
            var command = new AnswerQuestionCommand(attemptId, request.QuestionId, request.SelectedOptionId);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return result.Error.Code switch
                {
                    "403" => StatusCode(StatusCodes.Status403Forbidden, new { message = result.Error.Description }),
                    "409" => StatusCode(StatusCodes.Status409Conflict, new { message = result.Error.Description }),
                    "410" => StatusCode(StatusCodes.Status410Gone, new { message = result.Error.Description }),
                    "422" => StatusCode(StatusCodes.Status422UnprocessableEntity, new { message = result.Error.Description }),
                    _ => BadRequest(result.Error)
                };
            }

            return Ok(result.Value); // Returns 200 OK with { "saved": true }
        }

        [HttpGet("api/attempts/{attemptId}/timer")]
        [Authorize]
        public async Task<IActionResult> AttemptTimer(
           [FromRoute] Guid attemptId)
        {
            var result = await _mediator.Send(new GetQuizAttemptTimerQuery(attemptId));

            return Ok(result.Value);
        }

        [HttpPost("api/attempts/{attemptId}/submit")]
        [Authorize]
        public async Task<IActionResult> SubmitAttempt(
             SubmitQuizCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result.Value);
        }

        [HttpGet("api/attempts/{attemptId}/results")]
        [Authorize]
        public async Task<IActionResult> AttemptResults(
        [FromRoute] Guid attemptId)
        {
            var result = await _mediator.Send(new AttemptResultQuery(attemptId));

            return Ok(result.Value);
        }

    }
}
