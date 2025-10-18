using Heimlich.Application.DTOs;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Heimlich.Domain.Enums;

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
                .Where(e => e.State == SessionStateEnum.Active || e.State == SessionStateEnum.Validated)
                .AsQueryable();

            var rankings = await query
                .GroupBy(e => e.EvaluatedUserId)
                .Select(g => new PractitionerRankingDto
                {
                    UserId = g.Key,
                    AverageScore = g.Average(e => e.Score),
                    EvaluationCount = g.Count()
                }).ToListAsync(cancellationToken);

            return rankings;
        }
    }
}