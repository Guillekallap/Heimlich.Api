using System.Text.Json.Serialization;

namespace Heimlich.Application.DTOs
{
    public class CreateGroupDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("practitionerIds")]
        public List<string> PractitionerIds { get; set; }

        [JsonPropertyName("evaluationConfigId")]
        public int? EvaluationConfigId { get; set; } // opcional: vincular config existente, si null se usa Default
    }
}