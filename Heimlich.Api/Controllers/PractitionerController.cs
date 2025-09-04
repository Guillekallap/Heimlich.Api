using System.Security.Claims;
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
    [Route("api/practitioner")]
    [Authorize(Roles = "Practitioner")]
    public class PractitionerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PractitionerController(IMediator mediator) => _mediator = mediator;

        //// Ejecutar práctica de maniobra entrenamiento
        //[HttpPost("practice/training")]
        //public async Task<IActionResult> StartTraining([FromBody] CreatePracticeSessionDto dto)
        //{   
        //    // Obtener PractitionerId del token
        //    var practitionerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(practitionerId))
        //        return Unauthorized("No se pudo identificar al usuario practicante.");
        //    var result = await _mediator.Send(new CreatePracticeSessionCommand(dto, PracticeTypeEnum.Training, practitionerId));
        //    return Ok(result);
        //}

        // Ejecutar práctica de maniobra simulación
        [HttpPost("practice/simulation")]
        public async Task<IActionResult> StartSimulation([FromBody] CreatePracticeSessionDto dto)
        {
            // Validación de TrunkId
            if (dto.TrunkId <= 0)
                return BadRequest("Debe especificar un TrunkId válido para la simulación.");

            // Obtener PractitionerId del token
            var practitionerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(practitionerId))
                return Unauthorized("No se pudo identificar al usuario practicante.");

            if (dto.GroupId.HasValue) dto.GroupId = null;
            var command = new CreatePracticeSessionCommand(dto, PracticeTypeEnum.Simulation, practitionerId);
            var result = await _mediator.Send(command);
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
        public async Task<IActionResult> GetAssignedGroups([FromQuery] string userId)
        {
            var query = new GetAssignedGroupQuery(userId);
            var group = await _mediator.Send(query);
            return Ok(group);
        }
    }
}