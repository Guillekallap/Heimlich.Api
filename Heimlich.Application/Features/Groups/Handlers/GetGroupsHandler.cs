using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Queries;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Groups.Handlers
{
    public class GetGroupsHandler : IRequestHandler<GetGroupsQuery, IEnumerable<GroupDto>>
    {
        private readonly HeimlichDbContext _context;

        public GetGroupsHandler(HeimlichDbContext context) => _context = context;

        public async Task<IEnumerable<GroupDto>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Groups
                .Select(g => new GroupDto { Id = g.Id, Name = g.Name })
                .ToListAsync(cancellationToken);
        }
    }
}