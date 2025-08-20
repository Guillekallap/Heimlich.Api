using Heimlich.Application.DTOs;
using Heimlich.Application.Features.PracticeSessions.Queries;
using Heimlich.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.PracticeSessions.Handlers
{
    public class GetPracticeSessionsHandler : IRequestHandler<GetPracticeSessionsQuery, IEnumerable<PracticeSessionDto>>
    {
        private readonly HeimlichDbContext _context;

        public GetPracticeSessionsHandler(HeimlichDbContext context) => _context = context;

        public async Task<IEnumerable<PracticeSessionDto>> Handle(GetPracticeSessionsQuery request, CancellationToken cancellationToken)
        {
            return await _context.PracticeSessions
                .Select(s => new PracticeSessionDto
                {
                    Id = s.Id,
                    PracticeType = s.PracticeType,
                    CreationDate = s.CreationDate
                })
                .ToListAsync(cancellationToken);
        }
    }
}