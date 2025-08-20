using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Queries;
using Heimlich.Application.Features.PracticeSessions.Commands;
using Heimlich.Application.Features.PracticeSessions.Queries;
using Heimlich.Domain.Enums;
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

        // Ejecutar práctica de maniobra entrenamiento
        [HttpPost("practice/training")]
        public async Task<IActionResult> StartTraining([FromBody] CreatePracticeSessionDto dto)
        {
            var result = await _mediator.Send(new CreatePracticeSessionCommand(dto, PracticeTypeEnum.Training));
            return Ok(result);
        }

        // Ejecutar práctica de maniobra simulación
        [HttpPost("practice/simulation")]
        public async Task<IActionResult> StartSimulation([FromBody] CreatePracticeSessionDto dto)
        {
            var result = await _mediator.Send(new CreatePracticeSessionCommand(dto, PracticeTypeEnum.Simulation));
            return Ok(result);
        }

        // Cancelar práctica
        [HttpPost("practice/{sessionId}/cancel")]
        public async Task<IActionResult> CancelPractice(int sessionId)
        {
            var result = await _mediator.Send(new CancelPracticeSessionCommand(sessionId));
            return Ok(result);
        }

        // Visualizar prácticas de simulación
        [HttpGet("practice/simulation")]
        public async Task<IActionResult> GetSimulations([FromQuery] string userId)
        {
            var result = await _mediator.Send(new GetPracticeSessionsQuery(userId, PracticeTypeEnum.Simulation));
            return Ok(result);
        }

        // Obtener grupo asignado
        [HttpGet("groups/assigned")]
        public async Task<IActionResult> GetAssignedGroup([FromQuery] string userId)
        {
            var result = await _mediator.Send(new GetAssignedGroupQuery(userId));
            return Ok(result);
        }
    }
}