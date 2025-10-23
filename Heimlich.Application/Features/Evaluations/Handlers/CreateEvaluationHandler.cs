using AutoMapper;
using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class CreateEvaluationHandler : IRequestHandler<CreateEvaluationCommand, EvaluationDto>
    {
        private readonly HeimlichDbContext _context;
        private readonly IMapper _mapper;

        public CreateEvaluationHandler(HeimlichDbContext context, IMapper mapper)
        { _context = context; _mapper = mapper; }

        private void FillMeasurements(Evaluation evaluation, IEnumerable<EvaluationMeasurementInputDto> measurements)
        {
            if (measurements != null)
            {
                foreach (var m in measurements)
                {
                    evaluation.Measurements.Add(new Measurement
                    {
                        Evaluation = evaluation,
                        ElapsedMs = m.ElapsedMs,
                        ForceValue = m.ForceValue,
                        ForceIsValid = m.ForceIsValid,
                        TouchValue = m.TouchValue,
                        TouchIsValid = m.TouchIsValid,
                        HandPositionValue = m.HandPositionValue,
                        HandPositionIsValid = m.HandPositionIsValid,
                        PositionValue = m.PositionValue,
                        PositionIsValid = m.PositionIsValid,
                        IsValid = m.IsValid,
                        Time = DateTime.UtcNow.AddMilliseconds(m.ElapsedMs ?? 0)
                    });
                }
            }
        }

        // Simple normalization: if multiple measurements share the same ElapsedMs, pick the first one.
        private static List<EvaluationMeasurementInputDto> NormalizeMeasurementsSimple(List<EvaluationMeasurementInputDto> measurements)
        {
            if (measurements == null || measurements.Count == 0) return new List<EvaluationMeasurementInputDto>();

            var normalized = measurements
                .GroupBy(m => m.ElapsedMs ?? 0)
                .OrderBy(g => g.Key)
                .Select(g => g.First())
                .ToList();

            return normalized;
        }

        public async Task<EvaluationDto> Handle(CreateEvaluationCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var normalizedMeasurements = NormalizeMeasurementsSimple(dto.Measurements ?? new List<EvaluationMeasurementInputDto>());

            var evaluation = new Evaluation
            {
                EvaluatorId = request.EvaluatorId,
                EvaluatedUserId = dto.EvaluatedUserId,
                TrunkId = dto.TrunkId,
                GroupId = dto.GroupId,
                Comments = dto.Comments,
                Score = dto.Score ?? 0,
                State = SessionStateEnum.Active
            };

            FillMeasurements(evaluation, normalizedMeasurements);
            // Totals and success rate based on normalized measurements
            evaluation.TotalErrors = normalizedMeasurements.Count(m => !m.IsValid);
            evaluation.TotalSuccess = normalizedMeasurements.Count(m => m.IsValid);
            evaluation.TotalMeasurements = normalizedMeasurements.Count;
            evaluation.SuccessRate = evaluation.TotalMeasurements > 0 ? (double)evaluation.TotalSuccess / evaluation.TotalMeasurements : 0;
            _context.Evaluations.Add(evaluation);
            await _context.SaveChangesAsync(cancellationToken);
            return _mapper.Map<EvaluationDto>(evaluation);
        }
    }
}