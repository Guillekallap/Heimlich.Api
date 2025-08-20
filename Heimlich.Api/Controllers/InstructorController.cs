using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Commands;
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
    [Authorize(Roles = "Instructor")]
    public class InstructorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InstructorController(IMediator mediator) => _mediator = mediator;

        // Crear grupo
        [HttpPost("groups")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto dto)
        {
            var command = new CreateGroupCommand(dto.Name, dto.Description, dto.PractitionerIds);
            var group = await _mediator.Send(command);
            return Ok(group);
        }

        // Editar grupo
        [HttpPut("groups/{groupId}")]
        public async Task<IActionResult> EditGroup(int groupId, [FromBody] EditGroupDto dto)
        {
            var command = new EditGroupCommand(groupId, dto.Name, dto.Description, dto.PractitionerIds);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Eliminar grupo
        [HttpDelete("groups/{groupId}")]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            var command = new DeleteGroupCommand(groupId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Visualizar grupo asignado
        [HttpGet("groups/assigned")]
        public async Task<IActionResult> GetAssignedGroup([FromQuery] string userId)
        {
            var query = new GetAssignedGroupQuery(userId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Visualizar sesiones de práctica
        [HttpGet("practice-sessions")]
        public async Task<IActionResult> GetSessions([FromQuery] string userId)
        {
            var query = new GetPracticeSessionsQuery(userId, PracticeTypeEnum.Evaluation);
            var sessions = await _mediator.Send(query);
            return Ok(sessions);
        }

        // Ejecutar evaluación
        [HttpPost("practice/evaluation")]
        public async Task<IActionResult> StartEvaluation([FromBody] CreatePracticeSessionDto dto)
        {
            var command = new CreatePracticeSessionCommand(dto, PracticeTypeEnum.Evaluation);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Asignar practicante a grupo
        [HttpPost("groups/{groupId}/assign")]
        public async Task<IActionResult> AssignPractitionerToGroup(int groupId, [FromBody] AssignPractitionerDto dto)
        {
            var command = new AssignPractitionerToGroupCommand(groupId, dto.PractitionerId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Desasignar practicante de grupo
        [HttpPost("groups/{groupId}/remove")]
        public async Task<IActionResult> RemovePractitionerFromGroup(int groupId, [FromBody] RemovePractitionerDto dto)
        {
            var command = new RemovePractitionerFromGroupCommand(groupId, dto.PractitionerId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Visualizar evaluaciones
        [HttpGet("evaluations")]
        public async Task<IActionResult> GetEvaluations([FromQuery] int? groupId)
        {
            var query = new GetEvaluationsQuery(groupId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Validar evaluación
        [HttpPost("evaluations/{evaluationId}/validate")]
        public async Task<IActionResult> ValidateEvaluation(int evaluationId, [FromBody] ValidateEvaluationDto dto)
        {
            var command = new ValidateEvaluationCommand(evaluationId, dto.IsValid, dto.Comments);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Visualizar ranking
        [HttpGet("ranking")]
        public async Task<IActionResult> GetRanking([FromQuery] int? groupId)
        {
            var query = new GetRankingQuery(groupId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}