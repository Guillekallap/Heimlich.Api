using MediatR;

namespace Heimlich.Application.Features.Evaluations.Commands
{
    public class DeleteEvaluationConfigCommand : IRequest<bool>
    {
        public int GroupId { get; }

        public DeleteEvaluationConfigCommand(int groupId)
        {
            GroupId = groupId;
        }
    }
}