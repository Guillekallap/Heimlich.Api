using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Queries;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Groups.Handlers
{
    public class GetAssignedGroupHandler : IRequestHandler<GetAssignedGroupQuery, List<GroupDto>>
    {
        private readonly HeimlichDbContext _context;

        public GetAssignedGroupHandler(HeimlichDbContext context)
        {
            _context = context;
        }

        public async Task<List<GroupDto>> Handle(GetAssignedGroupQuery request, CancellationToken cancellationToken)
        {
            var userGroups = await _context.UserGroups
                .Include(ug => ug.Group)
                .Where(ug => ug.UserId == request.UserId)
                .Select(ug => ug.Group)
                .Include(g => g.UserGroups)
                .ToListAsync(cancellationToken);

            return userGroups.Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                PractitionerIds = g.UserGroups.Select(ug => ug.UserId).ToList()
            }).ToList();
        }
    }
}