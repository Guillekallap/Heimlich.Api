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
                        Time = DateTime.UtcNow.AddMilliseconds(m.ElapsedMs)
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