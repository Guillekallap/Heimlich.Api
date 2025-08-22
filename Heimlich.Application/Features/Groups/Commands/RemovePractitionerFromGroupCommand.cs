using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Groups.Commands
{
    public class RemovePractitionerFromGroupCommand : IRequest<GroupDto>
    {
        public int GroupId { get; }
        public string PractitionerId { get; }

        public RemovePractitionerFromGroupCommand(int groupId, string practitionerId)
        {
            GroupId = groupId;
            PractitionerId = practitionerId;
        }
    }
}