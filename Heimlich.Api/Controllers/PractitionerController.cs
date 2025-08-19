using Heimlich.Application.Features.PracticeSessions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Heimlich.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Practitioner")]
    public class PractitionerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PractitionerController(IMediator mediator) => _mediator = mediator;

        [HttpGet("practice-sessions")]
        public async Task<IActionResult> GetPracticeSessions()
        {
            var query = new GetPracticeSessionsQuery();
            var sessions = await _mediator.Send(query);
            return Ok(sessions);
        }

        // Aquí podrían agregarse endpoints adicionales (p.ej. reservar sesiones, ver grupos propios, etc.)
    }
}