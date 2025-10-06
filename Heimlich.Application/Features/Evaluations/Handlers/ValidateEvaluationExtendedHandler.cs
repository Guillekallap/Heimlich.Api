using System.Threading;
using System.Threading.Tasks;
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
            var evaluation = await _context.Evaluations.FirstOrDefaultAsync(e => e.Id == request.EvaluationId && e.EvaluatorId == request.EvaluatorId, cancellationToken);
            if (evaluation == null) return null;
            if (evaluation.State == SessionStateEnum.Cancelled)
                throw new InvalidOperationException("No se puede validar una evaluación cancelada.");
            if (evaluation.State == SessionStateEnum.Validated)
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
            evaluation.Score = request.Score;
            evaluation.IsValid = request.IsValid;
            evaluation.Comments = request.Comments;
            evaluation.Signature = request.Signature;
            evaluation.State = SessionStateEnum.Validated;
            evaluation.ValidatedAt = DateTime.UtcNow;
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