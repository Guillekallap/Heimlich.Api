using Microsoft.AspNetCore.Identity;

namespace Heimlich.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }
    }
}