using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Commands;
using Heimlich.Application.Features.Groups.Queries;
using Heimlich.Application.Features.PracticeSessions.Commands;
using Heimlich.Application.Features.PracticeSessions.Queries;
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

        [HttpPost("groups")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto dto)
        {
            var command = new CreateGroupCommand { Name = dto.Name };
            var group = await _mediator.Send(command);
            return Ok(group);
        }

        [HttpGet("groups")]
        public async Task<IActionResult> GetGroups()
        {
            var query = new GetGroupsQuery();
            var groups = await _mediator.Send(query);
            return Ok(groups);
        }

        [HttpPost("practice-sessions")]
        public async Task<IActionResult> CreateSession([FromBody] CreatePracticeSessionDto dto)
        {
            var command = new CreatePracticeSessionCommand
            {
                PracticeType = dto.PracticeType,
                CreationDate = dto.CreationDate
            };
            var session = await _mediator.Send(command);
            return Ok(session);
        }

        [HttpGet("practice-sessions")]
        public async Task<IActionResult> GetSessions()
        {
            var query = new GetPracticeSessionsQuery();
            var sessions = await _mediator.Send(query);
            return Ok(sessions);
        }
    }
}