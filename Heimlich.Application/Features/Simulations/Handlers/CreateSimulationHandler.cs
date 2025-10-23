using AutoMapper;
using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Simulations.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Heimlich.Application.Features.Simulations.Handlers
{
    public class CreateSimulationHandler : IRequestHandler<CreateSimulationCommand, SimulationSessionDto>
    {
        private readonly HeimlichDbContext _context;
        private readonly IMapper _mapper;

        public CreateSimulationHandler(HeimlichDbContext context, IMapper mapper)
        { _context = context; _mapper = mapper; }

        private void FillMeasurements(Simulation simulation, IEnumerable<SimulationSampleDto> samples)
        {
            if (samples != null)
            {
                foreach (var sample in samples)
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
        }

        private static List<SimulationSampleDto> NormalizeSamplesSimple(List<SimulationSampleDto> samples)
        {
            if (samples == null || samples.Count == 0) return new List<SimulationSampleDto>();

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

            // Normalizar samples para evitar duplicados (mantenemos la primera por elapsedMs)
            var normalizedSamples = NormalizeSamplesSimple(dto.Samples ?? new List<SimulationSampleDto>());

            var simulation = new Simulation
            {
                PractitionerId = request.PractitionerId,
                TrunkId = dto.TrunkId,
                State = SessionStateEnum.Active,
                Comments = dto.Comments ?? string.Empty
            };
            FillMeasurements(simulation, normalizedSamples);
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