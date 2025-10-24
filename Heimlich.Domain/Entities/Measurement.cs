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

        // Force value (string) and its validity/status
        public string? ForceValue { get; set; }

        public bool ForceStatus { get; set; }

        // Touch status (boolean)
        public bool TouchStatus { get; set; }

        // Angle in degrees (string) and its status
        public string? AngleDeg { get; set; }

        public bool AngleStatus { get; set; }

        // Position message and overall status
        public string? Message { get; set; }

        public bool Status { get; set; }

        // IsValid is true only if all metrics are valid
        public bool IsValid { get; set; }
    }
}