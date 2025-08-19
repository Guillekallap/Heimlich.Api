using Heimlich.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Infrastructure
{
    public class HeimlichDbContext : IdentityDbContext<User>
    {
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<PracticeType> PracticeTypes { get; set; }
        public DbSet<Trunk> Torsos { get; set; }
        public DbSet<PracticeSession> PracticeSessions { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Measurement> Measurements { get; set; }

        public HeimlichDbContext(DbContextOptions<HeimlichDbContext> options)
        : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserGroup>()
                .HasKey(ug => new { ug.UserId, ug.GroupId });
            builder.Entity<UserGroup>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UserGroups)
                .HasForeignKey(ug => ug.UserId);
            builder.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.UserGroups)
                .HasForeignKey(ug => ug.GroupId);

            builder.Entity<PracticeSession>()
                .HasOne(ps => ps.Practitioner)
                .WithMany(u => u.PracticeSessions)
                .HasForeignKey(ps => ps.PractitionerId);
            builder.Entity<Evaluation>()
                .HasOne(ev => ev.EvaluatedUser)
                .WithMany()
                .HasForeignKey(ev => ev.EvaluatedUserId);
            builder.Entity<Measurement>()
                .HasOne(m => m.PracticeSession)
                .WithMany(ps => ps.Measurements)
                .HasForeignKey(m => m.PracticeSessionId);
        }
    }
}