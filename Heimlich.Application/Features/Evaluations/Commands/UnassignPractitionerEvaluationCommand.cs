using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Evaluations.Commands
{
    public class UnassignPractitionerEvaluationCommand : IRequest<EvaluationDto>
    {
        public int EvaluationId { get; }

        public UnassignPractitionerEvaluationCommand(int evaluationId)
        {
            EvaluationId = evaluationId;
        }
    }
}