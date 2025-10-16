using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Groups.Commands
{
    public class CreateGroupCommand : IRequest<GroupDto>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> PractitionerIds { get; set; }
        public int? EvaluationConfigId { get; set; } // Soporte directo para config opcional

        public CreateGroupCommand(string name, string description, List<string> practitionerIds, int? evaluationConfigId = null)
        {
            Name = name;
            Description = description;
            PractitionerIds = practitionerIds ?? new List<string>();
            EvaluationConfigId = evaluationConfigId;
        }
    }
}