using Heimlich.Domain.Enums;

namespace Heimlich.Domain.Entities
{
    public class Measurement
    {
        public int Id { get; set; }
        public int? SimulationId { get; set; }
        public Simulation Simulation { get; set; }
        public int? EvaluationId { get; set; }
        public Evaluation Evaluation { get; set; }
        public DateTime Time { get; set; }
        public long? ElapsedMs { get; set; }
        public MetricTypeEnum MetricType { get; set; }
        public string Value { get; set; }
        public bool IsValid { get; set; }
    }
}