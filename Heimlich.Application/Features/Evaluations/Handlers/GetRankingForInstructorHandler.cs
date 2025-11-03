using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Queries;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class GetRankingForInstructorHandler : IRequestHandler<GetRankingForInstructorQuery, IEnumerable<GroupRankingDto>>
    {
        private readonly HeimlichDbContext _context;

        public GetRankingForInstructorHandler(HeimlichDbContext context)
        { _context = context; }

        public async Task<IEnumerable<GroupRankingDto>> Handle(GetRankingForInstructorQuery request, CancellationToken cancellationToken)
        {
            var evaluations = await _context.Evaluations
                .Include(e => e.EvaluatedUser)
                .Where(e => e.EvaluatorId == request.InstructorId && e.Score != null && e.GroupId != null && (e.State == Domain.Enums.SessionStateEnum.Active || e.State == Domain.Enums.SessionStateEnum.Validated))
                .ToListAsync(cancellationToken);

            var groupIds = evaluations.Select(e => e.GroupId.Value).Distinct().ToList();
            var groupNames = await _context.Groups.Where(g => groupIds.Contains(g.Id)).ToDictionaryAsync(g => g.Id, g => g.Name, cancellationToken);

            var groupData = evaluations
                .GroupBy(e => e.GroupId.Value)
                .Select(g => new GroupRankingDto
                {
                    GroupId = g.Key,
                    GroupName = groupNames.ContainsKey(g.Key) ? groupNames[g.Key] : "",
                    GroupAverage = g.Average(e => e.Score),
                    Practitioners = g.GroupBy(e => e.EvaluatedUserId).Select(pg => new PractitionerRankingDto
                    {
                        UserId = pg.Key,
                        FullName = pg.First().EvaluatedUser?.Fullname ?? "",
                        AverageScore = pg.Average(e => e.Score),
                        EvaluationCount = pg.Count()
                    }).ToList()
                })
                .ToList();

            return groupData;
        }
    }
}