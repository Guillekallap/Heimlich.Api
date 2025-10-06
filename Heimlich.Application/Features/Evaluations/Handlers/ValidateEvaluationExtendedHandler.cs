using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class ValidateEvaluationExtendedHandler : IRequestHandler<ValidateEvaluationExtendedCommand, EvaluationDto>
    {
        private readonly HeimlichDbContext _context;

        public ValidateEvaluationExtendedHandler(HeimlichDbContext context)
        { _context = context; }

        public async Task<EvaluationDto> Handle(ValidateEvaluationExtendedCommand request, CancellationToken cancellationToken)
        {
            var evaluation = await _context.Evaluations.FirstOrDefaultAsync(e => e.Id == request.EvaluationId, cancellationToken);
            if (evaluation == null) throw new KeyNotFoundException("Evaluación no encontrada");
            if (evaluation.EvaluatedUserId == null)
                throw new InvalidOperationException("No se puede validar una evaluación sin practicante asignado.");
            var config = await _context.EvaluationConfigs.FirstOrDefaultAsync(c => c.Id == request.EvaluationConfigId, cancellationToken);
            if (config == null)
                throw new KeyNotFoundException("Configuración de evaluación no encontrada.");
            evaluation.Score = request.Score;
            evaluation.IsValid = request.IsValid;
            evaluation.Comments = request.Comments;
            evaluation.Signature = request.Signature;
            evaluation.EvaluationConfigId = config.Id;
            evaluation.State = SessionStateEnum.Validated;
            evaluation.ValidatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            return new EvaluationDto
            {
                Id = evaluation.Id,
                EvaluatorId = evaluation.EvaluatorId,
                EvaluatedUserId = evaluation.EvaluatedUserId,
                TrunkId = evaluation.TrunkId,
                GroupId = evaluation.GroupId,
                EvaluationConfigId = evaluation.EvaluationConfigId,
                Score = evaluation.Score,
                Comments = evaluation.Comments,
                IsValid = evaluation.IsValid,
                State = evaluation.State
            };
        }
    }
}