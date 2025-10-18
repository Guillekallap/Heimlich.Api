using AutoMapper;
using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Simulations.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;
using System.Linq;

namespace Heimlich.Application.Features.Simulations.Handlers
{
    public class CreateSimulationHandler : IRequestHandler<CreateSimulationCommand, SimulationSessionDto>
    {
        private readonly HeimlichDbContext _context;
        private readonly IMapper _mapper;

        public CreateSimulationHandler(HeimlichDbContext context, IMapper mapper)
        { _context = context; _mapper = mapper; }

        private void FillMeasurements(Simulation simulation, CreateSimulationDto dto)
        {
            var orderedSamples = dto.Samples.OrderBy(s => s.ElapsedMs).ToList();
            foreach (var sample in orderedSamples)
            {
                var m = sample.Measurement;
                simulation.Measurements.Add(new Measurement
                {
                    Simulation = simulation,
                    ElapsedMs = sample.ElapsedMs,
                    ForceValue = m.ForceValue,
                    ForceIsValid = m.ForceIsValid,
                    TouchValue = m.TouchValue,
                    TouchIsValid = m.TouchIsValid,
                    HandPositionValue = m.HandPositionValue,
                    HandPositionIsValid = m.HandPositionIsValid,
                    PositionValue = m.PositionValue,
                    PositionIsValid = m.PositionIsValid,
                    IsValid = m.IsValid,
                    Time = DateTime.UtcNow.AddMilliseconds(sample.ElapsedMs)
                });
            }
        }

        public async Task<SimulationSessionDto> Handle(CreateSimulationCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            if (dto == null) throw new System.ArgumentNullException(nameof(request.Dto));
            if (dto.TrunkId <= 0) throw new System.ArgumentException("TrunkId inválido");
            if (string.IsNullOrEmpty(request.PractitionerId)) throw new System.ArgumentException("PractitionerId requerido");
            if (dto.Samples == null || dto.Samples.Count == 0) throw new System.ArgumentException("Samples requeridos");

            var simulation = new Simulation
            {
                PractitionerId = request.PractitionerId,
                TrunkId = dto.TrunkId,
                State = SessionStateEnum.Active,
                Comments = dto.Comments ?? string.Empty
            };
            FillMeasurements(simulation, dto);
            // Los totales y success rate los calcula y envía el mobile
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