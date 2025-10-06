using AutoMapper;
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
        private readonly IMapper _mapper;

        public CancelSimulationImmediateHandler(HeimlichDbContext context, IMapper mapper)
        { _context = context; _mapper = mapper; }

        private void FillMeasurements(Simulation simulation, CreateSimulationDto dto)
        {
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
        }

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
            FillMeasurements(simulation, dto);
            simulation.EndDate = DateTime.UtcNow;
            _context.Simulations.Add(simulation);
            await _context.SaveChangesAsync(cancellationToken);
            return _mapper.Map<SimulationSessionDto>(simulation);
        }
    }
}