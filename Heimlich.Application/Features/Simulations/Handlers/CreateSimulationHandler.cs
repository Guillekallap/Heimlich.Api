using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Simulations.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;

namespace Heimlich.Application.Features.Simulations.Handlers
{
    public class CreateSimulationHandler : IRequestHandler<CreateSimulationCommand, SimulationSessionDto>
    {
        private readonly HeimlichDbContext _context;
        public CreateSimulationHandler(HeimlichDbContext context) { _context = context; }

        public async Task<SimulationSessionDto> Handle(CreateSimulationCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            if (dto == null) throw new System.ArgumentNullException(nameof(request.Dto));
            if (dto.TrunkId <= 0) throw new System.ArgumentException("TrunkId inv�lido");
            if (string.IsNullOrEmpty(request.PractitionerId)) throw new System.ArgumentException("PractitionerId requerido");
            if (dto.Samples == null || dto.Samples.Count == 0) throw new System.ArgumentException("Samples requeridos");

            // Normalizar muestras ordenadas por ElapsedMs
            var orderedSamples = dto.Samples.OrderBy(s => s.ElapsedMs).ToList();

            var simulation = new Simulation
            {
                PractitionerId = request.PractitionerId,
                TrunkId = dto.TrunkId,
                State = SessionStateEnum.Active
            };

            foreach (var sample in orderedSamples)
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
                        Time = DateTime.UtcNow.AddMilliseconds(sample.ElapsedMs) // Aproximaci�n
                    });
                }
            }

            // Calcular agregados si no vienen en dto.Result
            var totalErrors = simulation.Measurements.Count(m => !m.IsValid);
            var duration = orderedSamples.Max(s => (long?)s.ElapsedMs) ?? 0;
            simulation.TotalErrors = dto.Result?.TotalErrors ?? totalErrors;
            simulation.TotalDurationMs = dto.Result?.TotalDurationMs ?? duration;
            simulation.AverageErrorsPerMeasurement = dto.Result?.AverageErrorsPerSample ?? (simulation.Measurements.Count == 0 ? 0 : (double)totalErrors / orderedSamples.Count);
            simulation.IsValid = dto.Result?.IsValid ?? (totalErrors == 0);
            simulation.Comments = dto.Result?.Comments;
            simulation.EndDate = simulation.TotalDurationMs.HasValue ? simulation.CreationDate.AddMilliseconds(simulation.TotalDurationMs.Value) : null;

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
