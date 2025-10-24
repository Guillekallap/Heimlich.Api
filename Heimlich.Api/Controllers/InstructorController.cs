using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Application.Features.Evaluations.Queries;
using Heimlich.Application.Features.Groups.Commands;
using Heimlich.Application.Features.Groups.Queries;
using Heimlich.Domain.Entities;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Heimlich.Api.Controllers
{
    [ApiController]
    [Route("api/instructor")]
    [Authorize(Roles = "Instructor")]
    public class InstructorController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly HeimlichDbContext _context;

        public InstructorController(IMediator mediator, HeimlichDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        // Crear grupo
        [HttpPost("groups")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto dto)
        {
            var command = new CreateGroupCommand(dto.Name, dto.Description, dto.PractitionerIds, dto.EvaluationConfigId);
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
            var command = new AssignPractitionerEvaluationCommand(evaluationId, userId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("evaluations/{evaluationId}/unassign-practitioner")]
        public async Task<IActionResult> UnassignPractitioner(int evaluationId)
        {
            var command = new UnassignPractitionerEvaluationCommand(evaluationId);
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

        // Eliminar configuración de evaluación no default -> soft-delete now
        [HttpDelete("evaluation-configs/{id}")]
        public async Task<IActionResult> DeleteEvaluationConfig(int id)
        {
            var config = await _context.EvaluationConfigs.Include(c => c.EvaluationConfigGroups).FirstOrDefaultAsync(c => c.Id == id);
            if (config == null) return NotFound();
            if (config.IsDefault) return BadRequest("No se puede eliminar la configuración default");
            if (config.EvaluationConfigGroups.Any()) return BadRequest("No se puede eliminar una configuración vinculada a grupos");

            // Soft-delete: mark inactive
            config.Status = Heimlich.Domain.Enums.EvaluationConfigStatusEnum.Inactive;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // CRUD EvaluationConfig
        [HttpPost("evaluation-configs")]
        public async Task<IActionResult> CreateEvaluationConfig([FromBody] EvaluationParametersDto dto)
        {
            var config = new EvaluationConfig
            {
                Name = dto.Name,
                MaxErrors = dto.MaxErrors,
                MaxTime = dto.MaxTime,
                MaxTimeInterval = dto.MaxTimeInterval,
                IsDefault = false
            };
            _context.EvaluationConfigs.Add(config);
            await _context.SaveChangesAsync();
            return Ok(config);
        }

        [HttpGet("evaluation-configs")]
        public async Task<IActionResult> GetEvaluationConfigs()
        {
            var configs = await _context.EvaluationConfigs
                .Where(c => c.Status == Heimlich.Domain.Enums.EvaluationConfigStatusEnum.Active)
                .ToListAsync();
            return Ok(configs);
        }

        [HttpPut("evaluation-configs/{id}")]
        public async Task<IActionResult> UpdateEvaluationConfig(int id, [FromBody] EvaluationParametersDto dto)
        {
            var config = await _context.EvaluationConfigs.FindAsync(id);
            if (config == null) return NotFound();
            if (config.IsDefault) return BadRequest("No se puede modificar la configuración default");
            config.Name = dto.Name;
            config.MaxErrors = dto.MaxErrors;
            config.MaxTime = dto.MaxTime;
            config.MaxTimeInterval = dto.MaxTimeInterval;
            await _context.SaveChangesAsync();
            return Ok(config);
        }

        // Listar practicantes simples
        [HttpGet("practitioners")]
        public async Task<IActionResult> GetPractitioners()
        {
            var practitioners = await _context.Users
                .Where(u => u.UserGroups.Any() && u.UserGroups.All(ug => ug.Group.Status == Heimlich.Domain.Enums.GroupStatusEnum.Active))
                .Select(u => new { u.Id, u.Fullname })
                .ToListAsync();
            return Ok(practitioners);
        }

        // Obtener evaluaciones por grupo y practicante
        [HttpGet("evaluations/by-group-practitioner")]
        public async Task<IActionResult> GetEvaluationsByGroupAndPractitioner([FromQuery] int groupId, [FromQuery] string userId)
        {
            var query = new GetEvaluationsQuery(groupId, userId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}