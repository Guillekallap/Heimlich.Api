using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Auth.Commands;
using Heimlich.Application.Features.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Heimlich.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("register"), AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var command = new RegisterCommand {Username = dto.UserName, Email = dto.Email, Password = dto.Password, Role = dto.Role};
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(Register), result);
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var query = new LoginQuery { Email = dto.Email, Password = dto.Password };
            var authResult = await _mediator.Send(query);
            if (authResult == null) return Unauthorized();
            return Ok(authResult);
        }
    }
}
