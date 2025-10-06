using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Queries;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class GetRankingForInstructorHandler : IRequestHandler<GetRankingForInstructorQuery, IEnumerable<RankingDto>>
    {
        private readonly HeimlichDbContext _context;

        public GetRankingForInstructorHandler(HeimlichDbContext context)
        { _context = context; }

        public async Task<IEnumerable<RankingDto>> Handle(GetRankingForInstructorQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Evaluations.Where(e => e.EvaluatorId == request.InstructorId && e.Score.HasValue);
            var ranking = await query
                .GroupBy(e => e.EvaluatedUserId)
                .Select(g => new RankingDto
                {
                    UserId = g.Key,
                    AverageScore = g.Average(e => e.Score ?? 0),
                    EvaluationCount = g.Count()
                })
                .OrderByDescending(r => r.AverageScore)
                .ToListAsync(cancellationToken);
            return ranking;
        }
    }
}