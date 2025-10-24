using System.Text.Json.Serialization;

namespace Heimlich.Application.DTOs
{
    public class CreateEvaluationDto
    {
        [JsonPropertyName("evaluatedUserId")]
        public string? EvaluatedUserId { get; set; }

        [JsonPropertyName("trunkId")]
        public int? TrunkId { get; set; }

        [JsonPropertyName("groupId")]
        public int? GroupId { get; set; }

        [JsonPropertyName("comments")]
        public string? Comments { get; set; }

        [JsonPropertyName("score")]
        public double? Score { get; set; }

        [JsonPropertyName("measurements")]
        public List<EvaluationMeasurementInputDto> Measurements { get; set; } = new();
    }

    public class EvaluationMeasurementInputDto
    {
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

    public class ValidateEvaluationExtendedDto
    {
        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("is_valid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("comments")]
        public string? Comments { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }

        [JsonPropertyName("evaluationConfigId")]
        public int EvaluationConfigId { get; set; }
    }
}