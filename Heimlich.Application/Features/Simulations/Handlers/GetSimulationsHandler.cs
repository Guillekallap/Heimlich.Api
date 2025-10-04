using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Simulations.Queries;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Simulations.Handlers
{
    public class GetSimulationsHandler : IRequestHandler<GetSimulationsQuery, IEnumerable<SimulationSessionDto>>
    {
        private readonly HeimlichDbContext _context;
        public GetSimulationsHandler(HeimlichDbContext context) { _context = context; }

        public async Task<IEnumerable<SimulationSessionDto>> Handle(GetSimulationsQuery request, CancellationToken cancellationToken)
        {
            var sims = await _context.Simulations
                .Where(s => s.PractitionerId == request.PractitionerId)
                .Include(s => s.Measurements)
                .OrderByDescending(s => s.CreationDate)
                .ToListAsync(cancellationToken);

            return sims.Select(sim => new SimulationSessionDto
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
            });
        }
    }
}
