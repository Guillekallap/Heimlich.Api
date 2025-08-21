using Heimlich.Application.DTOs;
using Heimlich.Domain.Enums;
using MediatR;

namespace Heimlich.Application.Features.Auth.Commands
{
    public class RegisterCommand : IRequest<RegisterResponseDto>
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public UserRoleEnum Role { get; set; }
    }
}