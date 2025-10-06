using AutoMapper;
using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class CreateEvaluationHandler : IRequestHandler<CreateEvaluationCommand, EvaluationDto>
    {
        private readonly HeimlichDbContext _context;
        private readonly IMapper _mapper;

        public CreateEvaluationHandler(HeimlichDbContext context, IMapper mapper)
        { _context = context; _mapper = mapper; }

        private void FillMeasurements(Evaluation evaluation, CreateEvaluationDto dto)
        {
            foreach (var m in dto.Measurements)
            {
                evaluation.Measurements.Add(new Measurement
                {
                    Evaluation = evaluation,
                    MetricType = m.MetricType,
                    Value = m.Value,
                    IsValid = m.IsValid,
                    ElapsedMs = m.ElapsedMs,
                    Time = DateTime.UtcNow.AddMilliseconds(m.ElapsedMs ?? 0)
                });
            }
        }

        public async Task<EvaluationDto> Handle(CreateEvaluationCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var evaluation = new Evaluation
            {
                EvaluatorId = request.EvaluatorId,
                EvaluatedUserId = dto.EvaluatedUserId,
                TrunkId = dto.TrunkId,
                Comments = dto.Comments,
                State = SessionStateEnum.Active
            };
            FillMeasurements(evaluation, dto);
            _context.Evaluations.Add(evaluation);
            await _context.SaveChangesAsync(cancellationToken);
            return _mapper.Map<EvaluationDto>(evaluation);
        }
    }
}