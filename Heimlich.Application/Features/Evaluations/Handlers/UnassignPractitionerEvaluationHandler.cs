using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class UnassignPractitionerEvaluationHandler : IRequestHandler<UnassignPractitionerEvaluationCommand, EvaluationDto>
    {
        private readonly HeimlichDbContext _context;

        public UnassignPractitionerEvaluationHandler(HeimlichDbContext context)
        { _context = context; }

        public async Task<EvaluationDto> Handle(UnassignPractitionerEvaluationCommand request, CancellationToken cancellationToken)
        {
            var eval = await _context.Evaluations.FirstOrDefaultAsync(e => e.Id == request.EvaluationId, cancellationToken);
            if (eval == null) throw new KeyNotFoundException("Evaluación no encontrada");
            if (eval.EvaluatedUserId == null)
                return ToDto(eval); // ya null
            eval.EvaluatedUserId = null; // asignar null coherente con nullable
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