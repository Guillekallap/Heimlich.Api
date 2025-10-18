using Heimlich.Domain.Enums;
using System.Collections.Generic;

namespace Heimlich.Application.DTOs
{
    public class EvaluationDto
    {
        public int Id { get; set; }
        public string EvaluatorId { get; set; }
        public string? EvaluatedUserId { get; set; }
        public int? TrunkId { get; set; }
        public int? GroupId { get; set; }
        public int? EvaluationConfigId { get; set; }
        public double Score { get; set; }
        public string? Comments { get; set; }
        public bool? IsValid { get; set; }
        public SessionStateEnum State { get; set; }
        public int TotalErrors { get; set; }
        public int TotalSuccess { get; set; }
        public int TotalMeasurements { get; set; }
        public double SuccessRate { get; set; }
        public IList<EvaluationMeasurementDto> Measurements { get; set; }
    }

    public class EvaluationMeasurementDto
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
}