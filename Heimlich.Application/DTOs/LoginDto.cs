using System.Text.Json.Serialization;

namespace Heimlich.Application.DTOs
{
    public class LoginUserDto
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; } = default!;

        [JsonPropertyName("password")]
        public string Password { get; set; } = default!;
    }
}