using MediatR;

namespace Heimlich.Application.Features.Groups.Commands
{
    public class DeleteGroupCommand : IRequest<Unit>
    {
        public int GroupId { get; }

        public DeleteGroupCommand(int groupId)
        {
            GroupId = groupId;
        }
    }
}