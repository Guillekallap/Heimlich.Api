namespace Heimlich.Application.Features.Groups.Commands
{
    public class DeleteGroupCommand
    {
        public int GroupId { get; set; }

        public DeleteGroupCommand(int groupId)
        {
            GroupId = groupId;
        }
    }
}