using Heimlich.Application.DTOs;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class ValidateEvaluationHandler : IRequestHandler<ValidateEvaluationCommand, EvaluationDto>
    {
        private readonly HeimlichDbContext _context;

        public ValidateEvaluationHandler(HeimlichDbContext context)
        {
            _context = context;
        }

        public async Task<EvaluationDto> Handle(ValidateEvaluationCommand request, CancellationToken cancellationToken)
        {
            var evaluation = await _context.Evaluations
                .FirstOrDefaultAsync(e => e.Id == request.EvaluationId, cancellationToken);

            if (evaluation == null) return null;

            evaluation.IsValid = request.IsValid;
            evaluation.Comments = request.Comments;
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