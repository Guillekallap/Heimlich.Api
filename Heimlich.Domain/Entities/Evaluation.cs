namespace Heimlich.Domain.Entities
{
    public class Evaluation
    {
        public int Id { get; set; }
        public int PracticeSessionId { get; set; }
        public PracticeSession PracticeSession { get; set; }
        public string EvaluatorId { get; set; }
        public User Evaluator { get; set; }
        public double Score { get; set; }
        public string Comments { get; set; }
    }
}