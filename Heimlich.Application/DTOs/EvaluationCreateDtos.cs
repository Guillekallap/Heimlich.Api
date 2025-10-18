using Heimlich.Domain.Enums;

namespace Heimlich.Application.DTOs
{
    public class CreateEvaluationDto
    {
        public string? EvaluatedUserId { get; set; }
        public int? TrunkId { get; set; }
        public int? GroupId { get; set; }
        public string? Comments { get; set; }
        public double? Score { get; set; }
        public List<EvaluationMeasurementInputDto> Measurements { get; set; } = new();
    }

    public class EvaluationMeasurementInputDto
    {
        public long? ElapsedMs { get; set; }
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

    public class ValidateEvaluationExtendedDto
    {
        public double Score { get; set; }
        public bool IsValid { get; set; }
        public string? Comments { get; set; }
        public string Signature { get; set; }
        public int EvaluationConfigId { get; set; }
    }
}