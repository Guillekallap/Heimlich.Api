namespace Heimlich.Domain.Entities
{
    public class PracticeType
    {
        public int Id { get; set; }
        public string Name { get; set; } // "Training" or "Simulation" or "Evaluation"
        public ICollection<PracticeSession> Sessions { get; set; }
    }
}