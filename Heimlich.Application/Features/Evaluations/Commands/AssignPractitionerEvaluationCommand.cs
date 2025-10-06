using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Evaluations.Commands
{
    public class AssignPractitionerEvaluationCommand : IRequest<EvaluationDto>
    {
        public int EvaluationId { get; }
        public string NewPractitionerUserId { get; }

        public AssignPractitionerEvaluationCommand(int evaluationId, string newPractitionerUserId)
        {
            EvaluationId = evaluationId;
            NewPractitionerUserId = newPractitionerUserId;
        }
    }
}