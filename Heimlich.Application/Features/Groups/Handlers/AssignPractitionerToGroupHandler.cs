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

            if (group == null)
                throw new InvalidOperationException("El grupo no existe.");

            var user = await _context.Users.FindAsync(request.PractitionerId);
            if (user == null)
                throw new InvalidOperationException("El practicante no existe.");

            if (group.UserGroups.Any(ug => ug.UserId == request.PractitionerId))
                throw new InvalidOperationException("El practicante ya está agregado al grupo.");

            group.UserGroups.Add(new UserGroup { GroupId = request.GroupId, UserId = request.PractitionerId });
            await _context.SaveChangesAsync(cancellationToken);

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