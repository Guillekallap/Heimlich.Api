using Heimlich.Application.DTOs;
using Heimlich.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class GetRankingHandler : IRequestHandler<GetRankingQuery, List<RankingDto>>
    {
        private readonly HeimlichDbContext _context;

        public GetRankingHandler(HeimlichDbContext context)
        {
            _context = context;
        }

        public async Task<List<RankingDto>> Handle(GetRankingQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Evaluations
                .Include(e => e.PracticeSession)
                .AsQueryable();

            if (request.GroupId.HasValue)
            {
                query = query.Where(e => e.PracticeSession.GroupId == request.GroupId.Value);
            }

            var rankings = await query
                .GroupBy(e => e.EvaluatedUserId)
                .Select(g => new RankingDto
                {
                    UserId = g.Key,
                    AverageScore = g.Average(e => e.Score),
                    EvaluationCount = g.Count()
                }).ToListAsync(cancellationToken);

            return rankings;
        }
    }
}
