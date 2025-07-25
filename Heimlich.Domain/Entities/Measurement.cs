namespace Heimlich.Domain.Entities
{
    public class Measurement
    {
        public int Id { get; set; }
        public int PracticeSessionId { get; set; }
        public PracticeSession PracticeSession { get; set; }
        public DateTime Time { get; set; }
        public string MetricType { get; set; }
        public double Value { get; set; }
    }
}