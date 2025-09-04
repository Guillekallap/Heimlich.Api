using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Groups.Commands
{
    public class AssignPractitionerToGroupCommand : IRequest<GroupDto>
    {
        public int GroupId { get; }
        public IList<string> PractitionerIds { get; }

        public AssignPractitionerToGroupCommand(int groupId, IList<string> practitionerIds)
        {
            GroupId = groupId;
            PractitionerIds = practitionerIds;
        }
    }
}