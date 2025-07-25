namespace Heimlich.Domain.Entities
{
    public class PracticeType
    {
        public int Id { get; set; }
        public string Name { get; set; } // "Training" or "Simulation"
        public ICollection<PracticeSession> Sessions { get; set; }

    }
}