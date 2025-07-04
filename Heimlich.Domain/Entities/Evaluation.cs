namespace Heimlich.Domain.Entities
{
    public class Evaluation
    {
        public int Id { get; set; }
        public DateTime EvaluationDate { get; set; }
        public bool IsApproved { get; set; }
        public string InstructorName { get; set; }

        public int PracticeSessionId { get; set; }
        public PracticeSession PracticeSession { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}