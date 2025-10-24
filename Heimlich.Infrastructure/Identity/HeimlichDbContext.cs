using Heimlich.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Infrastructure.Identity
{
    public class HeimlichDbContext : IdentityDbContext<User>
    {
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Trunk> Trunks { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<Simulation> Simulations { get; set; }
        public DbSet<EvaluationConfig> EvaluationConfigs { get; set; }
        public DbSet<EvaluationConfigGroup> EvaluationConfigGroups { get; set; }

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
                .HasForeignKey(ug => ug.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.UserGroups)
                .HasForeignKey(ug => ug.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Evaluation>()
                .Property(e => e.Comments).IsRequired(false);
            builder.Entity<Evaluation>()
                .Property(e => e.Signature).IsRequired(false);
            builder.Entity<Simulation>()
                .Property(s => s.Comments).IsRequired(false);

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

            builder.Entity<Evaluation>()
                .HasOne(ev => ev.Group)
                .WithMany()
                .HasForeignKey(ev => ev.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            // Preserve historical Evaluation.EvaluationConfigId references: prevent cascade set-null on config deletion
            builder.Entity<Evaluation>()
                .HasOne(ev => ev.EvaluationConfig)
                .WithMany()
                .HasForeignKey(ev => ev.EvaluationConfigId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Simulation>()
                .HasOne(s => s.Practitioner)
                .WithMany(u => u.Simulations)
                .HasForeignKey(s => s.PractitionerId)
                .OnDelete(DeleteBehavior.Restrict);

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

            // EvaluationConfig adjustments
            builder.Entity<EvaluationConfig>()
                .HasIndex(c => c.Name)
                .IsUnique();
            builder.Entity<EvaluationConfig>()
                .HasOne(ec => ec.Group)
                .WithMany()
                .HasForeignKey(ec => ec.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EvaluationConfigGroup>()
                .HasKey(ecg => new { ecg.EvaluationConfigId, ecg.GroupId });
            builder.Entity<EvaluationConfigGroup>()
                .HasOne(ecg => ecg.EvaluationConfig)
                .WithMany(c => c.EvaluationConfigGroups)
                .HasForeignKey(ecg => ecg.EvaluationConfigId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<EvaluationConfigGroup>()
                .HasOne(ecg => ecg.Group)
                .WithMany(g => g.EvaluationConfigGroups)
                .HasForeignKey(ecg => ecg.GroupId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}