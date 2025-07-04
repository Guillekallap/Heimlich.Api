namespace Heimlich.Domain.Entities
{
    public class Measurement
    {
        public int Id { get; set; }
        public decimal TotalTime { get; set; }
        public int TotalPoints { get; set; }
        public bool IsHandPositionCorrect { get; set; }
        public bool IsNearTorso { get; set; }
        public decimal StrengthPressure { get; set; }
        public DateTime Timestamp { get; set; }
        public int TorsoId { get; set; }

        public int PracticeSessionId { get; set; }
        public PracticeSession PracticeSession { get; set; }
    }
}