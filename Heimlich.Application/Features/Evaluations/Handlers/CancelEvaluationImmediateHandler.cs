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
                State = SessionStateEnum.Cancelled
            };
            FillMeasurements(evaluation, dto);
            evaluation.TotalErrors = dto.Measurements.Count(m => !m.IsValid);
            evaluation.TotalSuccess = dto.Measurements.Count(m => m.IsValid);
            evaluation.TotalMeasurements = dto.Measurements.Count;
            evaluation.SuccessRate = evaluation.TotalMeasurements > 0 ? (double)evaluation.TotalSuccess / evaluation.TotalMeasurements : 0;
            _context.Evaluations.Add(evaluation);
            await _context.SaveChangesAsync(cancellationToken);
            return _mapper.Map<EvaluationDto>(evaluation);
        }
    }
}