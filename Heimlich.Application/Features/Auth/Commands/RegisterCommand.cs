using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Auth.Commands
{
    public class RegisterCommand : IRequest<RegisterResponseDto>
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}