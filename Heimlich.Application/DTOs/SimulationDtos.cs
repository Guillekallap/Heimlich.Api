using Heimlich.Domain.Enums;

namespace Heimlich.Application.DTOs
{
    public class CreateSimulationDto
    {
        public int TrunkId { get; set; }
        public List<SimulationSampleDto> Samples { get; set; } = new();
        // Hacer opcional para que en cancel no sea requerido
        public SimulationResultDto? Result { get; set; }
    }

    public class SimulationSampleDto
    {
        public long ElapsedMs { get; set; }
        public List<SimulationMetricDto> Metrics { get; set; } = new();
    }

    public class SimulationMetricDto
    {
        public MetricTypeEnum MetricType { get; set; }
        public string Value { get; set; }
        public bool IsValid { get; set; }
    }

    public class SimulationResultDto
    {
        public bool? IsValid { get; set; }
        public long? TotalDurationMs { get; set; }
        public int? TotalErrors { get; set; }
        public double? AverageErrorsPerSample { get; set; }
        public string Comments { get; set; }
    }

    public class SimulationSessionDto
    {
        public int Id { get; set; }
        public int TrunkId { get; set; }
        public string PractitionerId { get; set; }
        public long? TotalDurationMs { get; set; }
        public int? TotalErrors { get; set; }
        public double? AverageErrorsPerSample { get; set; }
        public bool? IsValid { get; set; }
        public string Comments { get; set; }
        public IList<SimulationSampleDto> Samples { get; set; }
    }
}