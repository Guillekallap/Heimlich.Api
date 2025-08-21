using Heimlich.Domain.Enums;

namespace Heimlich.Domain.Entities
{
    public class PracticeSession
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string PractitionerId { get; set; }
        public User Practitioner { get; set; }
        public PracticeTypeEnum PracticeType { get; set; }
        public int? TrunkId { get; set; }
        public Trunk Trunk { get; set; }
        public int? GroupId { get; set; } // Solo se asigna si PracticeType == Evaluation
        public Group Group { get; set; }
        public ICollection<Measurement> Measurements { get; set; }
        public ICollection<Evaluation> Evaluations { get; set; }
    }
}