using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        public CreateEvaluationHandler(HeimlichDbContext context) { _context = context; }

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

            _context.Evaluations.Add(evaluation);
            await _context.SaveChangesAsync(cancellationToken);

            return new EvaluationDto
            {
                Id = evaluation.Id,
                EvaluatorId = evaluation.EvaluatorId,
                EvaluatedUserId = evaluation.EvaluatedUserId,
                Score = evaluation.Score,
                Comments = evaluation.Comments,
                IsValid = evaluation.IsValid,
                State = evaluation.State
            };
        }
    }
}
