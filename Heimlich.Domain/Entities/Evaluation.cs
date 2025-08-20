namespace Heimlich.Domain.Entities
{
    // Heimlich.Domain/Entities/Evaluation.cs
    public class Evaluation
    {
        public int Id { get; set; }
        public int PracticeSessionId { get; set; }
        public PracticeSession PracticeSession { get; set; }

        public string EvaluatorId { get; set; }
        public User Evaluator { get; set; }

        public string EvaluatedUserId { get; set; }
        public User EvaluatedUser { get; set; }

        public double Score { get; set; }
        public string Comments { get; set; }
        public bool IsValid { get; set; }
    }
}