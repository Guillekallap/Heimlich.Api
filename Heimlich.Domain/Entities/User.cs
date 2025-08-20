using Microsoft.AspNetCore.Identity;

namespace Heimlich.Domain.Entities
{
    public class User : IdentityUser
    {
        public string Fullname { get; set; } = default!;
        public string Password { get; set; } = default!;
        public DateTime CreationDate { get; set; }
        public int RoleId { get; set; }

        public ICollection<UserGroup> UserGroups { get; set; }
        public ICollection<PracticeSession> PracticeSessions { get; set; }
    }
}