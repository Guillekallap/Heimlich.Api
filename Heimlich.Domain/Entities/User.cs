namespace Heimlich.Domain.Entities
{
    public class User : IdentityUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public string Mail { get; set; } = default!;
        public string Fullname { get; set; } = default!;
        public string Password { get; set; } = default!;
        public DateTime CreationDate { get; set; }
        public int RoleId { get; set; }

        public ICollection<Group> Groups { get; set; } = new List<Group>();
        public ICollection<PracticeSession> PracticeSessions { get; set; } = new List<PracticeSession>();
        public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    }
}