using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Application.Features.Evaluations.Queries;
using Heimlich.Application.Features.Groups.Commands;
using Heimlich.Application.Features.Groups.Queries;
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
        [HttpDelete("groups/{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var command = new DeleteGroupCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }

        // Visualizar grupo asignado
        [HttpGet("groups/assigned")]
        public async Task<IActionResult> GetAssignedGroup([FromQuery] string userId)
        {
            var query = new GetAssignedGroupQuery(userId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Listar evaluaciones (todas las de grupos del instructor) - placeholder hasta implementar query específica
        [HttpGet("evaluations")]
        public async Task<IActionResult> GetEvaluations()
        {
            // Se asume query futura GetEvaluationsForInstructorQuery
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = new GetEvaluationsForInstructorQuery(instructorId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Crear evaluación
        [HttpPost("evaluations")]
        public async Task<IActionResult> CreateEvaluation([FromBody] CreateEvaluationDto dto)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(instructorId)) return Unauthorized();
            var command = new CreateEvaluationCommand(dto, instructorId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("evaluations/{evaluationId}/validate")]
        public async Task<IActionResult> ValidateEvaluation(int evaluationId, [FromBody] ValidateEvaluationExtendedDto dto)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(instructorId)) return Unauthorized();
            var command = new ValidateEvaluationExtendedCommand(evaluationId, instructorId, dto.Score, dto.IsValid, dto.Comments, dto.Signature);
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
            var command = new UpsertEvaluationConfigCommand(groupId, dto.MaxErrors, dto.MaxTime, false);
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
    }
}