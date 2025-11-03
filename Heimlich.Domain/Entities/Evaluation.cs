using Heimlich.Domain.Enums;

namespace Heimlich.Domain.Entities
{
    public class Evaluation
    {
        public int Id { get; set; }
        public string EvaluatorId { get; set; } // Instructor
        public User Evaluator { get; set; }
        public string? EvaluatedUserId { get; set; } // Practicante (puede ser null inicialmente en evaluación automática)
        public User? EvaluatedUser { get; set; }
        public int? TrunkId { get; set; }
        public Trunk? Trunk { get; set; }
        public int? GroupId { get; set; }
        public Group? Group { get; set; }
        public int? EvaluationConfigId { get; set; }
        public EvaluationConfig? EvaluationConfig { get; set; }
        public double Score { get; set; }
        public string? Comments { get; set; }
        public string? Signature { get; set; }
        public SessionStateEnum State { get; set; } = SessionStateEnum.Active;
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public DateTime? ValidatedAt { get; set; }
        public int TotalErrors { get; set; }
        public int TotalSuccess { get; set; }
        public int TotalMeasurements { get; set; }
        public double SuccessRate { get; set; }
        public long TotalDurationMs { get; set; }
        public double AverageErrorsPerMeasurement { get; set; }
        public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
    }
}