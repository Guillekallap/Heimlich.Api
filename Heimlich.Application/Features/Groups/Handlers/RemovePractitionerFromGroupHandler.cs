using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Commands;
using Heimlich.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Groups.Handlers
{
    public class RemovePractitionerFromGroupHandler : IRequestHandler<RemovePractitionerFromGroupCommand, GroupDto>
    {
        private readonly HeimlichDbContext _context;

        public RemovePractitionerFromGroupHandler(HeimlichDbContext context)
        {
            _context = context;
        }

        public async Task<GroupDto> Handle(RemovePractitionerFromGroupCommand request, CancellationToken cancellationToken)
        {
            var group = await _context.Groups
                .Include(g => g.UserGroups)
                .FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);

            if (group == null) return null;

            var userGroup = group.UserGroups.FirstOrDefault(ug => ug.UserId == request.PractitionerId);
            if (userGroup != null)
            {
                group.UserGroups.Remove(userGroup);
                await _context.SaveChangesAsync(cancellationToken);
            }

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
