using Heimlich.Application.DTOs;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class GetRankingHandler : IRequestHandler<GetRankingQuery, List<PractitionerRankingDto>>
    {
        private readonly HeimlichDbContext _context;

        public GetRankingHandler(HeimlichDbContext context)
        {
            _context = context;
        }

        public async Task<List<PractitionerRankingDto>> Handle(GetRankingQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Evaluations
                .Include(e => e.EvaluatedUser)
                .Where(e => e.State == SessionStateEnum.Active || e.State == SessionStateEnum.Validated)
                .AsQueryable();

            var rankings = await query
                .GroupBy(e => e.EvaluatedUserId)
                .Select(g => new PractitionerRankingDto
                {
                    UserId = g.Key,
                    FullName = g.First().EvaluatedUser != null ? g.First().EvaluatedUser.Fullname : "",
                    AverageScore = g.Average(e => e.Score),
                    EvaluationCount = g.Count()
                }).ToListAsync(cancellationToken);

            return rankings;
        }
    }
}