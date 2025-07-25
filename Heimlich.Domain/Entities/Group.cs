namespace Heimlich.Domain.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public string Status { get; set; }
        public ICollection<UserGroup> UserGroups { get; set; }

    }
}