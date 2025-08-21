using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Groups.Handlers
{
    public class AssignPractitionerToGroupHandler : IRequestHandler<AssignPractitionerToGroupCommand, GroupDto>
    {
        private readonly HeimlichDbContext _context;

        public AssignPractitionerToGroupHandler(HeimlichDbContext context)
        {
            _context = context;
        }

        public async Task<GroupDto> Handle(AssignPractitionerToGroupCommand request, CancellationToken cancellationToken)
        {
            var group = await _context.Groups
                .Include(g => g.UserGroups)
                .FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);

            if (group == null) return null;

            if (!group.UserGroups.Any(ug => ug.UserId == request.PractitionerId))
            {
                group.UserGroups.Add(new UserGroup { GroupId = request.GroupId, UserId = request.PractitionerId });
                await _context.SaveChangesAsync(cancellationToken);
            }

            // Mapear a GroupDto
            return new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                PractitionerIds = group.UserGroups.Select(ug => ug.UserId).ToList()
            };
        }
    }
}