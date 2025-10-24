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
                        // assign string values directly
                        ForceValue = m.ForceValue,
                        ForceStatus = m.ForceStatus,
                        TouchStatus = m.TouchStatus,
                        AngleDeg = m.AngleDeg,
                        AngleStatus = m.AngleStatus,
                        Message = m.Message,
                        Status = m.Status,
                        IsValid = m.IsValid,
                        Time = DateTimeOffset.FromUnixTimeMilliseconds(m.Timestamp).UtcDateTime
                    });
                }
            }
        }

        // Simple normalization: if multiple measurements share the same ElapsedMs, pick the first one.
        private static List<EvaluationMeasurementInputDto> NormalizeMeasurementsSimple(List<EvaluationMeasurementInputDto> measurements)
        {
            if (measurements == null || measurements.Count == 0) return new List<EvaluationMeasurementInputDto>();

            var normalized = measurements
                .GroupBy(m => m.ElapsedMs)
                .OrderBy(g => g.Key)
                .Select(g => g.First())
                .ToList();

            return normalized;
        }

        public async Task<EvaluationDto> Handle(CreateEvaluationCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var normalizedMeasurements = NormalizeMeasurementsSimple(dto.Measurements ?? new List<EvaluationMeasurementInputDto>());

            // determine evaluation config for this new evaluation
            int? evaluationConfigId = null;
            if (dto.GroupId.HasValue)
            {
                var link = await _context.EvaluationConfigGroups
                    .Include(l => l.EvaluationConfig)
                    .FirstOrDefaultAsync(l => l.GroupId == dto.GroupId.Value && l.EvaluationConfig.Status == EvaluationConfigStatusEnum.Active, cancellationToken);
                if (link != null)
                    evaluationConfigId = link.EvaluationConfigId;
            }

            if (evaluationConfigId == null)
            {
                var defaultConfig = await _context.EvaluationConfigs.FirstOrDefaultAsync(c => c.IsDefault && c.Status == EvaluationConfigStatusEnum.Active, cancellationToken);
                evaluationConfigId = defaultConfig?.Id;
            }

            var evaluation = new Evaluation
            {
                EvaluatorId = request.EvaluatorId,
                EvaluatedUserId = dto.EvaluatedUserId,
                TrunkId = dto.TrunkId,
                GroupId = dto.GroupId,
                EvaluationConfigId = evaluationConfigId,
                Comments = dto.Comments,
                Score = dto.Score ?? 0,
                State = SessionStateEnum.Active,
                TotalDurationMs = dto.TotalDurationMs ?? 0,
                TotalMeasurements = dto.TotalMeasurements ?? 0,
                TotalSuccess = dto.TotalSuccess ?? 0,
                TotalErrors = dto.TotalErrors ?? 0,
                SuccessRate = dto.SuccessRate ?? 0,
                AverageErrorsPerMeasurement = dto.AverageErrorsPerMeasurement ?? 0
            };

            FillMeasurements(evaluation, normalizedMeasurements);

            // If client didn't provide aggregates, compute them
            if (!dto.TotalMeasurements.HasValue)
                evaluation.TotalMeasurements = normalizedMeasurements.Count;
            if (!dto.TotalErrors.HasValue)
                evaluation.TotalErrors = normalizedMeasurements.Count(m => !m.IsValid);
            if (!dto.TotalSuccess.HasValue)
                evaluation.TotalSuccess = normalizedMeasurements.Count(m => m.IsValid);
            if (!dto.SuccessRate.HasValue)
                evaluation.SuccessRate = evaluation.TotalMeasurements > 0 ? (double)evaluation.TotalSuccess / evaluation.TotalMeasurements : 0;
            if (!dto.TotalDurationMs.HasValue)
                evaluation.TotalDurationMs = evaluation.Measurements.Any() ? evaluation.Measurements.Max(m => m.ElapsedMs ?? 0) : 0;
            if (!dto.AverageErrorsPerMeasurement.HasValue)
                evaluation.AverageErrorsPerMeasurement = evaluation.TotalMeasurements > 0 ? (double)evaluation.TotalErrors / evaluation.TotalMeasurements : 0;

            _context.Evaluations.Add(evaluation);
            await _context.SaveChangesAsync(cancellationToken);
            return _mapper.Map<EvaluationDto>(evaluation);
        }
    }
}