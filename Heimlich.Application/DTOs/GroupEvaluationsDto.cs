using System.Text.Json.Serialization;

namespace Heimlich.Application.DTOs
{
    public class GroupEvaluationsDto
    {
        [JsonPropertyName("groupId")]
        public int? GroupId { get; set; }

        [JsonPropertyName("groupName")]
        public string? GroupName { get; set; }

        [JsonPropertyName("evaluations")]
        public List<EvaluationDto> Evaluations { get; set; } = new List<EvaluationDto>();
    }
}
