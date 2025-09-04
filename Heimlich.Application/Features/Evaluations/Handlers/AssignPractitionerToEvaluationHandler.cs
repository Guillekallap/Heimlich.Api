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
                throw new InvalidOperationException("La evaluaci�n no existe.");

            // Validar que la lista no est� vac�a
            if (request.PractitionerIds == null || !request.PractitionerIds.Any())
                throw new InvalidOperationException("Debe especificar al menos un practicante para asignar.");

            var practitionerId = request.PractitionerIds.First();

            var user = await _context.Users.FindAsync(practitionerId);
            if (user == null)
                throw new InvalidOperationException("El practicante no existe.");

            if (evaluation.EvaluatedUserId == practitionerId)
                throw new InvalidOperationException("El practicante ya ha sido asignado a la evaluaci�n.");

            evaluation.EvaluatedUserId = practitionerId;
            await _context.SaveChangesAsync(cancellationToken);

            return new EvaluationDto
            {
                Id = evaluation.Id,
                PracticeSessionId = evaluation.PracticeSessionId,
                EvaluatorId = evaluation.EvaluatorId,
                EvaluatedUserId = evaluation.EvaluatedUserId,
                Score = evaluation.Score,
                Comments = evaluation.Comments,
                IsValid = evaluation.IsValid
            };
        }
    }
}