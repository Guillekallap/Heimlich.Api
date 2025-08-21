namespace Heimlich.Application.DTOs
{
    public class EvaluationDto
    {
        public int Id { get; set; }
        public int PracticeSessionId { get; set; }
        public string EvaluatorId { get; set; }
        public string EvaluatedUserId { get; set; }
        public double Score { get; set; }
        public string Comments { get; set; }
        public bool IsValid { get; set; }
    }
}