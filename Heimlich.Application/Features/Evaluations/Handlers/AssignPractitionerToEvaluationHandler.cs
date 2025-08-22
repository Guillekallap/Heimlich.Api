using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class AssignPractitionerToEvaluationHandler : IRequestHandler<AssignPractitionerToEvaluationCommand, EvaluationDto>
    {
        private readonly HeimlichDbContext _context;

        public AssignPractitionerToEvaluationHandler(HeimlichDbContext context)
        {
            _context = context;
        }

        public async Task<EvaluationDto> Handle(AssignPractitionerToEvaluationCommand request, CancellationToken cancellationToken)
        {
            var evaluation = await _context.Evaluations
                .Include(e => e.EvaluatedUser)
                .FirstOrDefaultAsync(e => e.Id == request.EvaluationId, cancellationToken);

            if (evaluation == null)
                throw new InvalidOperationException("La evaluación no existe.");

            var user = await _context.Users.FindAsync(request.PractitionerId);
            if (user == null)
                throw new InvalidOperationException("El practicante no existe.");

            if (evaluation.EvaluatedUserId == request.PractitionerId)
                throw new InvalidOperationException("El practicante ya ha sido asignado a la evaluación.");

            evaluation.EvaluatedUserId = request.PractitionerId;
            await _context.SaveChangesAsync(cancellationToken);

            return new EvaluationDto
            {
                Id = evaluation.Id,
                PracticeSessionId = evaluation.PracticeSessionId,
                EvaluatedUserId = evaluation.EvaluatedUserId,
                Score = evaluation.Score,
                Comments = evaluation.Comments,
                IsValid = evaluation.IsValid
            };
        }
    }
}