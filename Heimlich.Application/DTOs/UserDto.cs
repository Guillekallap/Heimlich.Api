namespace Heimlich.Application.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Mail { get; set; }
        public string Fullname { get; set; }
        public string Role { get; set; }
        public DateTime CreationDate { get; set; }
    }
}