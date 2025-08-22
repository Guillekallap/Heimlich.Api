using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Groups.Commands
{
    public class AssignPractitionerToGroupCommand : IRequest<GroupDto>
    {
        public int GroupId { get; set; }
        public string PractitionerId { get; set; }
        public AssignPractitionerToGroupCommand(int groupId, string practitionerId)
        {
            GroupId = groupId;
            PractitionerId = practitionerId;
        }
    }
}