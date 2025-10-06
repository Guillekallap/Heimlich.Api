using Heimlich.Domain.Entities;
using MediatR;

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