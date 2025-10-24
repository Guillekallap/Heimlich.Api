using System.Text.Json.Serialization;

namespace Heimlich.Application.DTOs
{
    public class AuthResultDto
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("expiry")]
        public DateTime Expiry { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
}