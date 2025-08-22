using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Commands;
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
        [HttpPost("groups/{groupId}/practitioners")]
        public async Task<IActionResult> AssignPractitioner(int groupId, [FromBody] AssignPractitionerDto dto)
        {
            var command = new AssignPractitionerToGroupCommand(groupId, dto.PractitionerId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Eliminar practicante de grupo
        [HttpDelete("groups/{groupId}/practitioners/{practitionerId}")]
        public async Task<IActionResult> RemovePractitioner(int groupId, string practitionerId)
        {
            var command = new RemovePractitionerFromGroupCommand(groupId, practitionerId);
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

        // Configurar parámetros de evaluación (intervalos por sensor, errores, tiempo)
        [HttpPut("groups/{groupId}/evaluation-parameters")]
        public async Task<IActionResult> UpdateEvaluationParameters(int groupId, [FromBody] EvaluationParametersDto dto)
        {
            var command = new UpdateEvaluationParametersCommand(groupId, dto.SensorIntervals, dto.MaxErrors, dto.MaxTime);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Restablecer parámetros de evaluación
        [HttpPost("groups/{groupId}/evaluation-parameters/reset")]
        public async Task<IActionResult> ResetEvaluationParameters(int groupId)
        {
            var command = new ResetEvaluationParametersCommand(groupId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Asignar practicante a evaluación
        [HttpPost("evaluations/{evaluationId}/practitioners")]
        public async Task<IActionResult> AssignPractitionerToEvaluation(int evaluationId, [FromBody] AssignPractitionerDto dto)
        {
            var command = new AssignPractitionerToEvaluationCommand(evaluationId, dto.PractitionerId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Desasignar practicante de evaluación
        [HttpDelete("evaluations/{evaluationId}/practitioners/{practitionerId}")]
        public async Task<IActionResult> RemovePractitionerFromEvaluation(int evaluationId, string practitionerId)
        {
            var command = new RemovePractitionerFromEvaluationCommand(evaluationId, practitionerId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}