using Heimlich.Application.DTOs;
using Heimlich.Infrastructure;
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
            var query = _context.Evaluations
                .Include(e => e.PracticeSession)
                .AsQueryable();

            if (request.GroupId.HasValue)
            {
                query = query.Where(e => e.PracticeSession.GroupId == request.GroupId.Value);
            }

            var evaluations = await query.ToListAsync(cancellationToken);

            return evaluations.Select(e => new EvaluationDto
            {
                Id = e.Id,
                PracticeSessionId = e.PracticeSessionId,
                EvaluatorId = e.EvaluatorId,
                EvaluatedUserId = e.EvaluatedUserId,
                Score = e.Score,
                Comments = e.Comments
            }).ToList();
        }
    }
}
