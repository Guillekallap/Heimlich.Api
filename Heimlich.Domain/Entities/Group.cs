namespace Heimlich.Domain.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public string Status { get; set; }
        public ICollection<UserGroup> UserGroups { get; set; }
        public ICollection<PracticeSession> PracticeSessions { get; set; } // Solo evaluaciones
    }
}