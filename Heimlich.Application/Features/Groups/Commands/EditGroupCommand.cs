using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Groups.Commands
{
    public class EditGroupCommand : IRequest<GroupDto>
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EvaluationDate { get; set; }
        public List<string> PractitionerIds { get; set; }

        public EditGroupCommand(int groupId, string name, string description, DateTime evaluationDate, List<string> practitionerIds)
        {
            GroupId = groupId;
            Name = name;
            Description = description;
            EvaluationDate = evaluationDate;
            PractitionerIds = practitionerIds ?? new List<string>();
        }
    }
}