using System.Text.Json.Serialization;

namespace Heimlich.Application.DTOs
{
    public class UserDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("mail")]
        public string Mail { get; set; }

        [JsonPropertyName("fullname")]
        public string Fullname { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("creationDate")]
        public DateTime CreationDate { get; set; }
    }
}