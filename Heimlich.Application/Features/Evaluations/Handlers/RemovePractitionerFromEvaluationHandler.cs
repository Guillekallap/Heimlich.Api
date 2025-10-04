using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class RemovePractitionerFromEvaluationHandler : IRequestHandler<RemovePractitionerFromEvaluationCommand, EvaluationDto>
    {
        private readonly HeimlichDbContext _context;

        public RemovePractitionerFromEvaluationHandler(HeimlichDbContext context)
        {
            _context = context;
        }

        public async Task<EvaluationDto> Handle(RemovePractitionerFromEvaluationCommand request, CancellationToken cancellationToken)
        {
            var evaluation = await _context.Evaluations
                .FirstOrDefaultAsync(e => e.Id == request.EvaluationId, cancellationToken);

            if (evaluation == null)
                throw new InvalidOperationException("La evaluación no existe.");

            if (evaluation.EvaluatedUserId != request.PractitionerId)
                throw new InvalidOperationException("El practicante ya ha sido desasignado o no pertenece a la evaluación.");

            evaluation.EvaluatedUserId = null; // permitir null temporalmente
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