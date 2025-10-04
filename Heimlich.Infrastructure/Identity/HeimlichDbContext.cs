using Heimlich.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Infrastructure.Identity
{
    public class HeimlichDbContext : IdentityDbContext<User>
    {
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Trunk> Torsos { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<Simulation> Simulations { get; set; }
        public DbSet<EvaluationConfig> EvaluationConfigs { get; set; }

        public HeimlichDbContext(DbContextOptions<HeimlichDbContext> options)
        : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Measurement>()
               .Property(m => m.MetricType)
               .HasConversion<int>();

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

            builder.Entity<Evaluation>()
                .HasOne(ev => ev.EvaluatedUser)
                .WithMany()
                .HasForeignKey(ev => ev.EvaluatedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Evaluation>()
                .HasOne(ev => ev.Evaluator)
                .WithMany(u => u.EvaluationsAuthored)
                .HasForeignKey(ev => ev.EvaluatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Simulation>()
                .HasOne(s => s.Practitioner)
                .WithMany(u => u.Simulations)
                .HasForeignKey(s => s.PractitionerId);

            builder.Entity<Measurement>()
                .HasOne(m => m.Simulation)
                .WithMany(s => s.Measurements)
                .HasForeignKey(m => m.SimulationId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Measurement>()
                .HasOne(m => m.Evaluation)
                .WithMany(e => e.Measurements)
                .HasForeignKey(m => m.EvaluationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Measurement>()
                .HasCheckConstraint("CK_Measurement_Owner", "(CASE WHEN [SimulationId] IS NOT NULL THEN 1 ELSE 0 END + CASE WHEN [EvaluationId] IS NOT NULL THEN 1 ELSE 0 END) = 1");

            builder.Entity<EvaluationConfig>()
                .HasOne(ec => ec.Group)
                .WithMany()
                .HasForeignKey(ec => ec.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}