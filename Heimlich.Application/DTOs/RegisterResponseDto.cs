using System.Text.Json.Serialization;

namespace Heimlich.Application.DTOs
{
    public class RegisterResponseDto
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
}