using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        await _context.SaveChangesAsync(cancellationToken);

        return new EvaluationDto
        {
            Id = evaluation.Id,
            PracticeSessionId = evaluation.PracticeSessionId,
            EvaluatorId = evaluation.EvaluatorId,
            EvaluatedUserId = evaluation.EvaluatedUserId,
            Score = evaluation.Score,
            Comments = evaluation.Comments
        };
    }
}