using Heimlich.Domain.Entities;
using MediatR;

namespace Heimlich.Application.Features.Evaluations.Commands
{
    public class SetGroupEvaluationConfigCommand : IRequest<EvaluationConfig>
    {
        public int GroupId { get; }
        public int EvaluationConfigId { get; }

        public SetGroupEvaluationConfigCommand(int groupId, int evaluationConfigId)
        {
            GroupId = groupId;
            EvaluationConfigId = evaluationConfigId;
        }
    }
}