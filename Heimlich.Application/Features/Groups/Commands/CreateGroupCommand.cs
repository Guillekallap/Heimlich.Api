using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Groups.Commands
{
    public class CreateGroupCommand : IRequest<GroupDto>
    {
        public string Name { get; set; }
    }
}