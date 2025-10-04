using Heimlich.Domain.Enums;

namespace Heimlich.Domain.Entities
{
    public class Evaluation
    {
        public int Id { get; set; }
        public string EvaluatorId { get; set; } // Instructor
        public User Evaluator { get; set; }
        public string EvaluatedUserId { get; set; } // Practicante
        public User EvaluatedUser { get; set; }
        public int? TrunkId { get; set; }
        public Trunk Trunk { get; set; }
        public double? Score { get; set; }
        public string Comments { get; set; }
        public bool? IsValid { get; set; }
        public string Signature { get; set; }
        public SessionStateEnum State { get; set; } = SessionStateEnum.Active;
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public DateTime? ValidatedAt { get; set; }
        public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
    }
}