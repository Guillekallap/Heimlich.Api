namespace Heimlich.Application.DTOs
{
    public class CreateSimulationDto
    {
        public int TrunkId { get; set; }
        public List<SimulationSampleDto> Samples { get; set; } = new();
        public string? Comments { get; set; }
    }

    public class SimulationSampleDto
    {
        public long ElapsedMs { get; set; }
        public SimulationMeasurementDto Measurement { get; set; }
    }

    public class SimulationMeasurementDto
    {
        public string ForceValue { get; set; }
        public bool ForceIsValid { get; set; }
        public string TouchValue { get; set; }
        public bool TouchIsValid { get; set; }
        public string HandPositionValue { get; set; }
        public bool HandPositionIsValid { get; set; }
        public string PositionValue { get; set; }
        public bool PositionIsValid { get; set; }
        public bool IsValid { get; set; }
    }

    public class SimulationSessionDto
    {
        public int Id { get; set; }
        public int TrunkId { get; set; }
        public string PractitionerId { get; set; }
        public long TotalDurationMs { get; set; }
        public int TotalErrors { get; set; }
        public int TotalSuccess { get; set; }
        public int TotalMeasurements { get; set; }
        public double SuccessRate { get; set; }
        public double AverageErrorsPerMeasurement { get; set; }
        public bool? IsValid { get; set; }
        public string Comments { get; set; }
        public IList<SimulationSampleDto> Samples { get; set; }
    }
}