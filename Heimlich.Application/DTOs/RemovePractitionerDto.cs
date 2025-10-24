using System.Text.Json.Serialization;

namespace Heimlich.Application.DTOs
{
    public class RemovePractitionerDto
    {
        [JsonPropertyName("practitionerId")]
        public string PractitionerId { get; set; }
    }
}