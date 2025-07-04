namespace Heimlich.Domain.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public DateTime CreationDate { get; set; }
        public string Status { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}