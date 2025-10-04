namespace Heimlich.Domain.Entities
{
    public class Trunk
    {
        public int Id { get; set; }
        public string Description { get; set; } // e.g. "Adult Mannequin"
        public ICollection<Simulation> Simulations { get; set; }
        public ICollection<Evaluation> Evaluations { get; set; }
    }
}