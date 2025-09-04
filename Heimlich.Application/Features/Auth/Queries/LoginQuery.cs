using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Auth.Queries
{
    public class LoginQuery : IRequest<AuthResultDto>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}