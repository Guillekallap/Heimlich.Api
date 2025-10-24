using AutoMapper;
using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class CancelEvaluationImmediateHandler : IRequestHandler<CancelEvaluationImmediateCommand, EvaluationDto>
    {
        private readonly HeimlichDbContext _context;
        private readonly IMapper _mapper;

        public CancelEvaluationImmediateHandler(HeimlichDbContext context, IMapper mapper)
        { _context = context; _mapper = mapper; }

        private void FillMeasurements(Evaluation evaluation, CreateEvaluationDto dto)
        {
            if (dto.Measurements != null)
            {
                foreach (var m in dto.Measurements)
                {
                    evaluation.Measurements.Add(new Measurement
                    {
                        Evaluation = evaluation,
                        ElapsedMs = m.ElapsedMs,
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

        public async Task<EvaluationDto> Handle(CancelEvaluationImmediateCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var evaluation = new Evaluation
            {
                EvaluatorId = request.EvaluatorId,
                EvaluatedUserId = dto.EvaluatedUserId,
                TrunkId = dto.TrunkId,
                GroupId = dto.GroupId,
                Comments = dto.Comments,
                State = SessionStateEnum.Cancelled,

                // accept client-provided aggregates if present, otherwise compute later
                TotalDurationMs = dto.TotalDurationMs ?? 0,
                TotalMeasurements = dto.TotalMeasurements ?? 0,
                TotalSuccess = dto.TotalSuccess ?? 0,
                TotalErrors = dto.TotalErrors ?? 0,
                SuccessRate = dto.SuccessRate ?? 0,
                AverageErrorsPerMeasurement = dto.AverageErrorsPerMeasurement ?? 0
            };

            FillMeasurements(evaluation, dto);

            // If client didn't provide aggregates, compute them
            if (!dto.TotalMeasurements.HasValue)
                evaluation.TotalMeasurements = evaluation.Measurements.Count;
            if (!dto.TotalErrors.HasValue)
                evaluation.TotalErrors = evaluation.Measurements.Count(m => !m.IsValid);
            if (!dto.TotalSuccess.HasValue)
                evaluation.TotalSuccess = evaluation.Measurements.Count(m => m.IsValid);
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