using Heimlich.Domain.Enums;

namespace Heimlich.Application.DTOs
{
    public class RegisterUserDto
    {
        public string UserName { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public UserRoleEnum Role { get; set; }
    }
}