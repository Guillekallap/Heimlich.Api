using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Evaluations.Commands
{
    public class RemovePractitionerFromEvaluationCommand : IRequest<EvaluationDto>
    {
        public int EvaluationId { get; }
        public string PractitionerId { get; }

        public RemovePractitionerFromEvaluationCommand(int evaluationId, string practitionerId)
        {
            EvaluationId = evaluationId;
            PractitionerId = practitionerId;
        }
    }
}