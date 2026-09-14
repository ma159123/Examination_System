using Application.CQRS.Features.Student.Diplomas;
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
        [HttpPost("diplomas")]
        [Authorize]
        public async Task<IActionResult> GetAllCatalogDiplomas(GetCatalogDiplomasQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
