using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Simulations.Queries;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Simulations.Handlers
{
    public class GetSimulationsHandler : IRequestHandler<GetSimulationsQuery, IEnumerable<SimulationSessionDto>>
    {
        private readonly HeimlichDbContext _context;

        public GetSimulationsHandler(HeimlichDbContext context)
        { _context = context; }

        public async Task<IEnumerable<SimulationSessionDto>> Handle(GetSimulationsQuery request, CancellationToken cancellationToken)
        {
            var sims = await _context.Simulations
                .Where(s => s.PractitionerId == request.PractitionerId && s.State != SessionStateEnum.Cancelled)
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
                TotalSuccess = sim.TotalSuccess,
                TotalMeasurements = sim.TotalMeasurements,
                SuccessRate = sim.SuccessRate,
                AverageErrorsPerMeasurement = sim.AverageErrorsPerMeasurement,
                IsValid = sim.IsValid,
                Comments = sim.Comments,
                Samples = sim.Measurements
                    .OrderBy(m => m.ElapsedMs)
                    .Select(m => new SimulationSampleDto
                    {
                        ElapsedMs = m.ElapsedMs ?? 0,
                        Measurement = new SimulationMeasurementDto
                        {
                            ForceValue = m.ForceValue,
                            ForceIsValid = m.ForceIsValid,
                            TouchValue = m.TouchValue,
                            TouchIsValid = m.TouchIsValid,
                            HandPositionValue = m.HandPositionValue,
                            HandPositionIsValid = m.HandPositionIsValid,
                            PositionValue = m.PositionValue,
                            PositionIsValid = m.PositionIsValid,
                            IsValid = m.IsValid
                        }
                    }).ToList()
            });
        }
    }
}