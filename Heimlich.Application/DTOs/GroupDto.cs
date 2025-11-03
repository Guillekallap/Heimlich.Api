using System.Text.Json.Serialization;

namespace Heimlich.Application.DTOs
{
    public class GroupDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonPropertyName("evaluationDate")]
        public DateTime EvaluationDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("ownerInstructorId")]
        public string OwnerInstructorId { get; set; }

        [JsonPropertyName("ownerInstructorName")]
        public string OwnerInstructorName { get; set; }

        [JsonPropertyName("practitionerIds")]
        public IList<string> PractitionerIds { get; set; }
    }
}