using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Heimlich.Application.DTOs
{
    public class AssignPractitionerDto
    {
        [JsonPropertyName("practitionerIds")]
        public IList<string> PractitionerIds { get; set; }
    }
}