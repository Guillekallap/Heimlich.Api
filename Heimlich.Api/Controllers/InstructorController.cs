using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Application.Features.Evaluations.Queries;
using Heimlich.Application.Features.Groups.Commands;
using Heimlich.Application.Features.Groups.Queries;
using Heimlich.Application.Features.Groups.Handlers; // for extended command
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Heimlich.Api.Controllers
{
    [ApiController]
    [Route("api/instructor")]
    [Authorize(Roles = "Instructor")]
    public class InstructorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InstructorController(IMediator mediator) => _mediator = mediator;

        // Crear grupo
        [HttpPost("groups")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto dto)
        {
            var command = new CreateGroupCommandWithConfig(dto.Name, dto.Description, dto.PractitionerIds, dto.EvaluationConfigId);
            var group = await _mediator.Send(command);
            return Ok(group);
        }

        // Obtener grupos creados por el instructor
        [HttpGet("groups/owned")]
        public async Task<IActionResult> GetOwnedGroups()
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = new GetOwnedGroupsQuery(instructorId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Configurar evaluación para el grupo
        [HttpPost("groups/{groupId}/config/{configId}")]
        public async Task<IActionResult> SetGroupConfig(int groupId, int configId)
        {
            var cmd = new SetGroupEvaluationConfigCommand(groupId, configId);
            var result = await _mediator.Send(cmd);
            return Ok(result);
        }

        // Editar grupo
        [HttpPut("groups/{groupId}")]
        public async Task<IActionResult> EditGroup(int groupId, [FromBody] EditGroupDto dto)
        {
            var command = new EditGroupCommand(groupId, dto.Name, dto.Description, dto.PractitionerIds);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Inactivar grupo (soft delete)
        [HttpDelete("groups/{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var command = new DeleteGroupCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }

        // Visualizar grupos asignados
        [HttpGet("groups/assigned")]
        public async Task<IActionResult> GetAssignedGroup([FromQuery] string userId)
        {
            var query = new GetAssignedGroupQuery(userId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("evaluations")]
        public async Task<IActionResult> GetEvaluations()
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = new GetEvaluationsForInstructorQuery(instructorId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("evaluations/create")]
        public async Task<IActionResult> CreateEvaluation([FromBody] CreateEvaluationDto dto)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(instructorId)) return Unauthorized();
            var command = new CreateEvaluationCommand(dto, instructorId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("evaluations/cancel")]
        public async Task<IActionResult> CancelEvaluation([FromBody] CreateEvaluationDto dto)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(instructorId)) return Unauthorized();
            var command = new CancelEvaluationImmediateCommand(dto, instructorId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("evaluations/{evaluationId}/validate")]
        public async Task<IActionResult> ValidateEvaluation(int evaluationId, [FromBody] ValidateEvaluationExtendedDto dto)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(instructorId)) return Unauthorized();
            var command = new ValidateEvaluationExtendedCommand(evaluationId, instructorId, dto.Score, dto.IsValid, dto.Comments, dto.Signature, dto.EvaluationConfigId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("evaluations/{evaluationId}/assign-practitioner/{userId}")]
        public async Task<IActionResult> AssignPractitioner(int evaluationId, string userId)
        {
            var command = new Heimlich.Application.Features.Evaluations.Commands.AssignPractitionerEvaluationCommand(evaluationId, userId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("evaluations/{evaluationId}/unassign-practitioner")]
        public async Task<IActionResult> UnassignPractitioner(int evaluationId)
        {
            var command = new Heimlich.Application.Features.Evaluations.Commands.UnassignPractitionerEvaluationCommand(evaluationId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Ranking por grupos del instructor
        [HttpGet("ranking")]
        public async Task<IActionResult> GetRanking()
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = new GetRankingForInstructorQuery(instructorId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Actualizar parámetros de evaluación
        [HttpPut("groups/{groupId}/evaluation-parameters")]
        public async Task<IActionResult> UpdateEvaluationParameters(int groupId, [FromBody] EvaluationParametersDto dto)
        {
            var command = new UpsertEvaluationConfigCommand(groupId, dto.MaxErrors, dto.MaxTime, dto.Name, false);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Restablecer parámetros de evaluación a default
        [HttpPost("groups/{groupId}/evaluation-parameters/reset")]
        public async Task<IActionResult> ResetEvaluationParameters(int groupId)
        {
            var command = new ResetEvaluationConfigCommand(groupId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Eliminar configuración de evaluación no default
        [HttpDelete("groups/{groupId}/evaluation-parameters")]
        public async Task<IActionResult> DeleteEvaluationParameters(int groupId)
        {
            var command = new DeleteEvaluationConfigCommand(groupId);
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}