using MediatR;
using Heimlich.Domain.Entities;

namespace Heimlich.Application.Features.Evaluations.Commands
{
    public class ResetEvaluationConfigCommand : IRequest<EvaluationConfig>
    {
        public int GroupId { get; }
        public ResetEvaluationConfigCommand(int groupId)
        {
            GroupId = groupId;
        }
    }
}
