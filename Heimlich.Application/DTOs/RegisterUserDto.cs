using System.Text.Json.Serialization;
using Heimlich.Domain.Enums;

namespace Heimlich.Application.DTOs
{
    public class RegisterUserDto
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; } = default!;

        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = default!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = default!;

        [JsonPropertyName("password")]
        public string Password { get; set; } = default!;

        [JsonPropertyName("role")]
        public UserRoleEnum Role { get; set; }
    }
}