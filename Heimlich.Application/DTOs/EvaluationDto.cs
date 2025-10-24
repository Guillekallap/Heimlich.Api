using Heimlich.Domain.Enums;
using System.Text.Json.Serialization;

namespace Heimlich.Application.DTOs
{
    public class EvaluationDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("evaluatorId")]
        public string EvaluatorId { get; set; }

        [JsonPropertyName("evaluatedUserId")]
        public string? EvaluatedUserId { get; set; }

        [JsonPropertyName("trunkId")]
        public int? TrunkId { get; set; }

        [JsonPropertyName("groupId")]
        public int? GroupId { get; set; }

        [JsonPropertyName("evaluationConfigId")]
        public int? EvaluationConfigId { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("comments")]
        public string? Comments { get; set; }

        [JsonPropertyName("is_valid")]
        public bool? IsValid { get; set; }

        [JsonPropertyName("state")]
        public SessionStateEnum State { get; set; }

        [JsonPropertyName("totalErrors")]
        public int TotalErrors { get; set; }

        [JsonPropertyName("totalSuccess")]
        public int TotalSuccess { get; set; }

        [JsonPropertyName("totalMeasurements")]
        public int TotalMeasurements { get; set; }

        [JsonPropertyName("successRate")]
        public double SuccessRate { get; set; }

        [JsonPropertyName("measurements")]
        public IList<EvaluationMeasurementDto> Measurements { get; set; }
    }

    public class EvaluationMeasurementDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("elapsedMs")]
        public long? ElapsedMs { get; set; }

        [JsonPropertyName("force_value")]
        public string ForceValue { get; set; }

        [JsonPropertyName("force_status")]
        public bool ForceIsValid { get; set; }

        [JsonPropertyName("touch_value")]
        public string TouchValue { get; set; }

        [JsonPropertyName("touch_status")]
        public bool TouchIsValid { get; set; }

        [JsonPropertyName("hand_position_value")]
        public string HandPositionValue { get; set; }

        [JsonPropertyName("hand_position_status")]
        public bool HandPositionIsValid { get; set; }

        [JsonPropertyName("position_value")]
        public string PositionValue { get; set; }

        [JsonPropertyName("position_is_valid")]
        public bool PositionIsValid { get; set; }

        [JsonPropertyName("is_valid")]
        public bool IsValid { get; set; }
    }
}