using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Queries;
using Heimlich.Application.Features.Simulations.Commands;
using Heimlich.Application.Features.Simulations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Heimlich.Api.Controllers
{
    [ApiController]
    [Route("api/practitioner")]
    [Authorize(Roles = "Practitioner")]
    public class PractitionerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PractitionerController(IMediator mediator) => _mediator = mediator;

        [HttpPost("simulations/create")]
        public async Task<IActionResult> CreateSimulation([FromBody] CreateSimulationDto dto)
        {
            if (dto.TrunkId <= 0)
                return BadRequest("Debe especificar un TrunkId válido.");

            var practitionerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(practitionerId))
                return Unauthorized("No se pudo identificar al usuario practicante.");

            var command = new CreateSimulationCommand(dto, practitionerId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("simulations/cancel")]
        public async Task<IActionResult> CancelSimulation([FromBody] CreateSimulationDto dto)
        {
            if (dto.TrunkId <= 0)
                return BadRequest("Debe especificar un TrunkId válido.");

            var practitionerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(practitionerId))
                return Unauthorized();

            var command = new CancelSimulationImmediateCommand(dto, practitionerId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("simulations")]
        public async Task<IActionResult> GetSimulations()
        {
            var practitionerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(practitionerId))
                return Unauthorized();
            var query = new GetSimulationsQuery(practitionerId);
            var list = await _mediator.Send(query);
            return Ok(list);
        }

        [HttpGet("groups/assigned")]
        public async Task<IActionResult> GetAssignedGroups([FromQuery] string userId)
        {
            var query = new GetAssignedGroupQuery(userId);
            var group = await _mediator.Send(query);
            return Ok(group);
        }
    }
}