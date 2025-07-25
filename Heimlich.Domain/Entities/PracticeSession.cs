namespace Heimlich.Domain.Entities
{
    public class PracticeSession
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string PractitionerId { get; set; }
        public User Practitioner { get; set; }
        public int PracticeTypeId { get; set; }
        public PracticeType PracticeType { get; set; }
        public int? TrunkId { get; set; }
        public Trunk Trunk { get; set; }
        public ICollection<Measurement> Measurements { get; set; }
        public ICollection<Evaluation> Evaluations { get; set; }

    }
}