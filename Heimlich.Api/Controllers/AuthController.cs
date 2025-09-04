using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Auth.Commands;
using Heimlich.Application.Features.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Heimlich.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register"), AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var command = new RegisterCommand
            {
                UserName = dto.UserName,
                Email = dto.Email,
                Password = dto.Password,
                FullName = dto.FullName,
                Role = dto.Role
            };
            var result = await _mediator.Send(command);
            if (!string.IsNullOrEmpty(result?.Error))
                return BadRequest(new { message = result.Error });
            return CreatedAtAction(nameof(Register), result);
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            var query = new LoginQuery { UserName = dto.UserName, Password = dto.Password };
            var authResult = await _mediator.Send(query);
            if (!string.IsNullOrEmpty(authResult?.Error))
                return Unauthorized(new { message = authResult.Error });
            return Ok(authResult);
        }

        [HttpPost("logout"), Authorize]
        public async Task<IActionResult> Logout()
        {
            var result = await _mediator.Send(new LogoutCommand());
            if (result)
                return Ok(new { message = "Sesión cerrada correctamente" });
            return BadRequest(new { message = "Error al cerrar sesión" });
        }
    }
}