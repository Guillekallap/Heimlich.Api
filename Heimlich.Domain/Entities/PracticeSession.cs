namespace Heimlich.Domain.Entities
{
    public class PracticeSession
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public int PracticeTypeId { get; set; }
        public PracticeType PracticeType { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
        public Evaluation Evaluation { get; set; }
    }
}