using AutoMapper;
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
        private readonly IMapper _mapper;

        public CreateSimulationHandler(HeimlichDbContext context, IMapper mapper)
        { _context = context; _mapper = mapper; }

        private void FillMeasurements(Simulation simulation, IEnumerable<SimulationMeasurementDto> measurements)
        {
            if (measurements != null)
            {
                foreach (var m in measurements)
                {
                    simulation.Measurements.Add(new Measurement
                    {
                        Simulation = simulation,
                        ElapsedMs = m.ElapsedMs,
                        ForceValue = m.ForceValue,
                        ForceStatus = m.ForceStatus,
                        TouchStatus = m.TouchStatus,
                        AngleDeg = m.AngleDeg,
                        AngleStatus = m.AngleStatus,
                        Message = m.Message ?? string.Empty,
                        Status = m.Status,
                        IsValid = m.IsValid,
                        Time = DateTime.UtcNow.AddMilliseconds(m.ElapsedMs)
                    });
                }
            }
        }

        private static List<SimulationMeasurementDto> NormalizeSamplesSimple(List<SimulationMeasurementDto> samples)
        {
            if (samples == null || samples.Count == 0) return new List<SimulationMeasurementDto>();

            var normalized = samples
                .GroupBy(s => s.ElapsedMs)
                .OrderBy(g => g.Key)
                .Select(g => g.First())
                .ToList();

            return normalized;
        }

        public async Task<SimulationSessionDto> Handle(CreateSimulationCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            if (dto == null) throw new System.ArgumentNullException(nameof(request.Dto));
            if (dto.TrunkId <= 0) throw new System.ArgumentException("TrunkId inválido");
            if (string.IsNullOrEmpty(request.PractitionerId)) throw new System.ArgumentException("PractitionerId requerido");

            var normalizedMeasurements = NormalizeSamplesSimple(dto.Measurements ?? new List<SimulationMeasurementDto>());

            var simulation = new Simulation
            {
                PractitionerId = request.PractitionerId,
                TrunkId = dto.TrunkId,
                State = SessionStateEnum.Active,
                Comments = dto.Comments ?? string.Empty
            };
            FillMeasurements(simulation, normalizedMeasurements);

            simulation.TotalErrors = simulation.Measurements.Count(m => !m.IsValid);
            simulation.TotalSuccess = simulation.Measurements.Count(m => m.IsValid);
            simulation.TotalMeasurements = simulation.Measurements.Count;
            simulation.SuccessRate = simulation.TotalMeasurements > 0 ? (double)simulation.TotalSuccess / simulation.TotalMeasurements : 0;
            simulation.TotalDurationMs = simulation.Measurements.Any() ? simulation.Measurements.Max(m => m.ElapsedMs ?? 0) : 0;
            simulation.AverageErrorsPerMeasurement = simulation.TotalMeasurements > 0 ? (double)simulation.TotalErrors / simulation.TotalMeasurements : 0;
            _context.Simulations.Add(simulation);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<SimulationSessionDto>(simulation);
        }
    }
}