using Heimlich.Application.DTOs;
using Heimlich.Infrastructure.Identity;
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
            var query = _context.Evaluations.AsQueryable();

            var rankings = await query
                .GroupBy(e => e.EvaluatedUserId)
                .Select(g => new RankingDto
                {
                    UserId = g.Key,
                    AverageScore = g.Average(e => e.Score ?? 0),
                    EvaluationCount = g.Count()
                }).ToListAsync(cancellationToken);

            return rankings;
        }
    }
}