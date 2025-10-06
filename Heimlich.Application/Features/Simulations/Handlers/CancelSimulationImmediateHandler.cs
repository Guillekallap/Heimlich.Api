using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Simulations.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;

namespace Heimlich.Application.Features.Simulations.Handlers
{
    public class CancelSimulationImmediateHandler : IRequestHandler<CancelSimulationImmediateCommand, SimulationSessionDto>
    {
        private readonly HeimlichDbContext _context;

        public CancelSimulationImmediateHandler(HeimlichDbContext context)
        { _context = context; }

        public async Task<SimulationSessionDto> Handle(CancelSimulationImmediateCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            if (dto.TrunkId <= 0) throw new System.ArgumentException("TrunkId inválido");
            var simulation = new Simulation
            {
                PractitionerId = request.PractitionerId,
                TrunkId = dto.TrunkId,
                State = SessionStateEnum.Cancelled
            };
            var ordered = dto.Samples?.OrderBy(s => s.ElapsedMs).ToList() ?? new();
            foreach (var sample in ordered)
            {
                foreach (var metric in sample.Metrics)
                {
                    simulation.Measurements.Add(new Measurement
                    {
                        Simulation = simulation,
                        MetricType = metric.MetricType,
                        Value = metric.Value,
                        IsValid = metric.IsValid,
                        ElapsedMs = sample.ElapsedMs,
                        Time = DateTime.UtcNow.AddMilliseconds(sample.ElapsedMs)
                    });
                }
            }
            simulation.TotalErrors = simulation.Measurements.Count(m => !m.IsValid);
            simulation.TotalDurationMs = ordered.Count == 0 ? 0 : ordered.Max(s => (long?)s.ElapsedMs) ?? 0;
            simulation.AverageErrorsPerMeasurement = ordered.Count == 0 ? 0 : (double)simulation.TotalErrors / ordered.Count;
            simulation.IsValid = simulation.TotalErrors == 0;
            simulation.EndDate = DateTime.UtcNow;
            _context.Simulations.Add(simulation);
            await _context.SaveChangesAsync(cancellationToken);

            return new SimulationSessionDto
            {
                Id = simulation.Id,
                PractitionerId = simulation.PractitionerId,
                TrunkId = simulation.TrunkId,
                TotalDurationMs = simulation.TotalDurationMs,
                TotalErrors = simulation.TotalErrors,
                AverageErrorsPerSample = simulation.AverageErrorsPerMeasurement,
                IsValid = simulation.IsValid,
                Comments = simulation.Comments,
                Samples = simulation.Measurements
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
}