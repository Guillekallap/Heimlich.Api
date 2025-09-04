using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Evaluations.Commands
{
    public class AssignPractitionerToEvaluationCommand : IRequest<EvaluationDto>
    {
        public int EvaluationId { get; }
        public IList<string> PractitionerIds { get; }

        public AssignPractitionerToEvaluationCommand(int evaluationId, IList<string> practitionerIds)
        {
            EvaluationId = evaluationId;
            PractitionerIds = practitionerIds;
        }
    }
}