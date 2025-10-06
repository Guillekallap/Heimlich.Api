using Heimlich.Domain.Enums;

namespace Heimlich.Application.DTOs
{
    public class CreateEvaluationDto
    {
        public string? EvaluatedUserId { get; set; }
        public int? TrunkId { get; set; }
        public int? GroupId { get; set; }
        public string? Comments { get; set; }
        public List<EvaluationMeasurementInputDto> Measurements { get; set; } = new();
    }

    public class EvaluationMeasurementInputDto
    {
        public long? ElapsedMs { get; set; }
        public MetricTypeEnum MetricType { get; set; }
        public string Value { get; set; }
        public bool IsValid { get; set; }
    }

    public class ValidateEvaluationExtendedDto
    {
        public double Score { get; set; }
        public bool IsValid { get; set; }
        public string? Comments { get; set; }
        public string Signature { get; set; }
        public int EvaluationConfigId { get; set; }
    }
}