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

        [JsonPropertyName("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonPropertyName("validatedAt")]
        public DateTime? ValidatedAt { get; set; }

        [JsonPropertyName("totalErrors")]
        public int TotalErrors { get; set; }

        [JsonPropertyName("totalSuccess")]
        public int TotalSuccess { get; set; }

        [JsonPropertyName("totalMeasurements")]
        public int TotalMeasurements { get; set; }

        [JsonPropertyName("successRate")]
        public double SuccessRate { get; set; }

        [JsonPropertyName("totalDurationMs")]
        public long TotalDurationMs { get; set; }

        [JsonPropertyName("averageErrorsPerMeasurement")]
        public double AverageErrorsPerMeasurement { get; set; }

        [JsonPropertyName("measurements")]
        public IList<EvaluationMeasurementDto> Measurements { get; set; } = new List<EvaluationMeasurementDto>();
    }

    // Align with SimulationMeasurementDto
    public class EvaluationMeasurementDto
    {
        // optional measurement id
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("elapsedMs")]
        public long ElapsedMs { get; set; }

        [JsonPropertyName("result")]
        public string Result { get; set; }

        [JsonPropertyName("angle_deg")]
        public string AngleDeg { get; set; }

        [JsonPropertyName("angle_status")]
        public bool AngleStatus { get; set; }

        [JsonPropertyName("force_value")]
        public string ForceValue { get; set; }

        [JsonPropertyName("force_status")]
        public bool ForceStatus { get; set; }

        [JsonPropertyName("touch_status")]
        public bool TouchStatus { get; set; }

        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("is_valid")]
        public bool IsValid { get; set; }
    }
}