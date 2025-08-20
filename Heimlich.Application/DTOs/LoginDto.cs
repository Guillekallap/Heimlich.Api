namespace Heimlich.Application.DTOs
{
    public class LoginUserDto
    {
        public string Email { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}