using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Simulations.Commands;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Simulations.Handlers
{
    public class CancelSimulationHandler : IRequestHandler<CancelSimulationCommand, SimulationSessionDto>
    {
        private readonly HeimlichDbContext _context;
        public CancelSimulationHandler(HeimlichDbContext context) { _context = context; }

        public async Task<SimulationSessionDto> Handle(CancelSimulationCommand request, CancellationToken cancellationToken)
        {
            var sim = await _context.Simulations
                .Include(s => s.Measurements)
                .FirstOrDefaultAsync(s => s.Id == request.SimulationId && s.PractitionerId == request.PractitionerId, cancellationToken);
            if (sim == null) return null;
            if (sim.State == SessionStateEnum.Cancelled || sim.State == SessionStateEnum.Validated)
                return ToDto(sim);
            sim.State = SessionStateEnum.Cancelled;
            sim.EndDate = DateTime.UtcNow;
            if (!sim.TotalDurationMs.HasValue)
            {
                var maxElapsed = sim.Measurements.Max(m => m.ElapsedMs) ?? 0;
                sim.TotalDurationMs = maxElapsed;
            }
            await _context.SaveChangesAsync(cancellationToken);
            return ToDto(sim);
        }

        private SimulationSessionDto ToDto(Heimlich.Domain.Entities.Simulation sim) => new SimulationSessionDto
        {
            Id = sim.Id,
            PractitionerId = sim.PractitionerId,
            TrunkId = sim.TrunkId,
            TotalDurationMs = sim.TotalDurationMs,
            TotalErrors = sim.TotalErrors,
            AverageErrorsPerSample = sim.AverageErrorsPerMeasurement,
            IsValid = sim.IsValid,
            Comments = sim.Comments,
            Samples = sim.Measurements
                .GroupBy(m => m.ElapsedMs ?? 0)
                .Select(g => new SimulationSampleDto
                {
                    ElapsedMs = g.Key,
                    Metrics = g.Select(m => new SimulationMetricDto
                    {
                        MetricType = m.MetricType,
                        Value = m.Value,
                        IsValid = m.IsValid
                    }).ToList()
                }).ToList()
        };
    }
}
