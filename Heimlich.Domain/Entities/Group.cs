using Heimlich.Domain.Enums;

namespace Heimlich.Domain.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public GroupStatusEnum Status { get; set; } = GroupStatusEnum.Active;
        public string OwnerInstructorId { get; set; }
        public User? OwnerInstructor { get; set; }
        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
        public ICollection<EvaluationConfigGroup> EvaluationConfigGroups { get; set; } = new List<EvaluationConfigGroup>();
    }
}