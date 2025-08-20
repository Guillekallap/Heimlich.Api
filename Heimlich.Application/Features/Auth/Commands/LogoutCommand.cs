using MediatR;

namespace Heimlich.Application.Features.Auth.Commands
{
    public class LogoutCommand : IRequest<bool>
    { }
}