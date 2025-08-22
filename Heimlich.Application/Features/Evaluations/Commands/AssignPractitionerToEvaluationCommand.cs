using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Evaluations.Commands
{
    public class AssignPractitionerToEvaluationCommand : IRequest<EvaluationDto>
    {
        public int EvaluationId { get; }
        public string PractitionerId { get; }

        public AssignPractitionerToEvaluationCommand(int evaluationId, string practitionerId)
        {
            EvaluationId = evaluationId;
            PractitionerId = practitionerId;
        }
    }
}