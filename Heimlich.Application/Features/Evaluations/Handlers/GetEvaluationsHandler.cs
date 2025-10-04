using Heimlich.Application.DTOs;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class GetEvaluationsHandler : IRequestHandler<GetEvaluationsQuery, List<EvaluationDto>>
    {
        private readonly HeimlichDbContext _context;

        public GetEvaluationsHandler(HeimlichDbContext context)
        {
            _context = context;
        }

        public async Task<List<EvaluationDto>> Handle(GetEvaluationsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Evaluations.AsQueryable();

            if (request.GroupId.HasValue)
            {
                // TODO: filtrar por grupo cuando Evaluation tenga relación directa a Group si se requiere
            }

            var evaluations = await query.ToListAsync(cancellationToken);

            return evaluations.Select(e => new EvaluationDto
            {
                Id = e.Id,
                EvaluatorId = e.EvaluatorId,
                EvaluatedUserId = e.EvaluatedUserId,
                Score = e.Score,
                Comments = e.Comments,
                IsValid = e.IsValid,
                State = e.State
            }).ToList();
        }
    }
}
