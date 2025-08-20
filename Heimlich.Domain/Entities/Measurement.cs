using Heimlich.Domain.Enums;

namespace Heimlich.Domain.Entities
{
    public class Measurement
    {
        public int Id { get; set; }
        public int PracticeSessionId { get; set; }
        public PracticeSession PracticeSession { get; set; }
        public DateTime Time { get; set; }
        public MetricTypeEnum MetricType { get; set; }
        public string Value { get; set; }
        public bool IsValid { get; set; }

    }
}