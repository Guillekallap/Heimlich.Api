using System.Text.Json.Serialization;

namespace Heimlich.Application.DTOs
{
    public class EditGroupDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("practitionerIds")]
        public List<string> PractitionerIds { get; set; }
    }
}