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
            var groups = await _context.UserGroups
                .Where(ug => ug.UserId == request.UserId)
                .Select(ug => ug.Group)
                .Distinct()
                .ToListAsync(cancellationToken);

            var groupIds = groups.Select(g => g.Id).ToList();
            var userGroupsByGroup = await _context.UserGroups
                .Where(ug => groupIds.Contains(ug.GroupId))
                .GroupBy(ug => ug.GroupId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.UserId).ToList(), cancellationToken);

            // Obtener nombres de owner
            var ownerIds = groups.Select(g => g.OwnerInstructorId).Where(id => id != null).Distinct().ToList();
            var owners = await _context.Users
                .Where(u => ownerIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Fullname, cancellationToken);

            return groups.Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                CreationDate = g.CreationDate,
                Status = g.Status.ToString(),
                OwnerInstructorId = g.OwnerInstructorId,
                OwnerInstructorName = g.OwnerInstructorId != null && owners.ContainsKey(g.OwnerInstructorId) ? owners[g.OwnerInstructorId] : null,
                PractitionerIds = userGroupsByGroup.ContainsKey(g.Id) ? userGroupsByGroup[g.Id] : new List<string>()
            }).ToList();
        }
    }
}