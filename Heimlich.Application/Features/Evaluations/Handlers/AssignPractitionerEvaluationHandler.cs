using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class AssignPractitionerEvaluationHandler : IRequestHandler<AssignPractitionerEvaluationCommand, EvaluationDto>
    {
        private readonly HeimlichDbContext _context;

        public AssignPractitionerEvaluationHandler(HeimlichDbContext context)
        { _context = context; }

        public async Task<EvaluationDto> Handle(AssignPractitionerEvaluationCommand request, CancellationToken cancellationToken)
        {
            var eval = await _context.Evaluations.FirstOrDefaultAsync(e => e.Id == request.EvaluationId, cancellationToken);
            if (eval == null) throw new KeyNotFoundException("Evaluación no encontrada");
            if (!await _context.Users.AnyAsync(u => u.Id == request.NewPractitionerUserId, cancellationToken))
                throw new KeyNotFoundException("Usuario practicante no existe");
            if (eval.EvaluatedUserId == request.NewPractitionerUserId)
                return ToDto(eval); // ya asignado
            eval.EvaluatedUserId = request.NewPractitionerUserId;
            await _context.SaveChangesAsync(cancellationToken);
            return ToDto(eval);
        }

        private static EvaluationDto ToDto(Domain.Entities.Evaluation e) => new EvaluationDto
        {
            Id = e.Id,
            EvaluatorId = e.EvaluatorId,
            EvaluatedUserId = e.EvaluatedUserId,
            GroupId = e.GroupId,
            EvaluationConfigId = e.EvaluationConfigId,
            TrunkId = e.TrunkId,
            Score = e.Score,
            Comments = e.Comments,
            IsValid = e.IsValid,
            State = e.State
        };
    }
}