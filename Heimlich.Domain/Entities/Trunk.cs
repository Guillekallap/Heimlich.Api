namespace Heimlich.Domain.Entities
{
    public class Trunk
    {
        public int Id { get; set; }
        public string Description { get; set; } // e.g. "Adult Mannequin"
        public ICollection<PracticeSession> Sessions { get; set; }
    }
}
