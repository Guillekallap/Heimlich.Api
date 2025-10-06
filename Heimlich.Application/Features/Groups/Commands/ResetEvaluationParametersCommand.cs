using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Groups.Commands
{
    public class ResetEvaluationParametersCommand : IRequest<GroupDto>
    {
        public int GroupId { get; }

        public ResetEvaluationParametersCommand(int groupId)
        {
            GroupId = groupId;
        }
    }
}