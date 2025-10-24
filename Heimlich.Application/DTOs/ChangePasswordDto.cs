using System.Text.Json.Serialization;

namespace Heimlich.Application.DTOs
{
    public class ChangePasswordDto
    {
        [JsonPropertyName("currentPassword")]
        public string CurrentPassword { get; set; }

        [JsonPropertyName("newPassword")]
        public string NewPassword { get; set; }
    }
}