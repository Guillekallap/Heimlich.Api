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
            var ordered = dto.Measurements.OrderBy(s => s.ElapsedMs).ToList();
            foreach (var m in ordered)
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

        public async Task<SimulationSessionDto> Handle(CancelSimulationImmediateCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            if (dto == null) throw new System.ArgumentNullException(nameof(request.Dto));
            if (dto.TrunkId <= 0) throw new System.ArgumentException("TrunkId inválido");
            if (string.IsNullOrEmpty(request.PractitionerId)) throw new System.ArgumentException("PractitionerId requerido");
            if (dto.Measurements == null || dto.Measurements.Count == 0) throw new System.ArgumentException("Measurements requeridos");

            var simulation = new Simulation
            {
                PractitionerId = request.PractitionerId,
                TrunkId = dto.TrunkId,
                State = SessionStateEnum.Cancelled,
                Comments = dto.Comments ?? string.Empty
            };
            FillMeasurements(simulation, dto);
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