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

            foreach (var practitionerId in request.PractitionerIds)
            {
                var user = await _context.Users.FindAsync(practitionerId);
                if (user == null)
                    throw new InvalidOperationException($"El practicante con ID {practitionerId} no existe.");

                if (group.UserGroups.Any(ug => ug.UserId == practitionerId))
                    continue; // Ya está agregado, lo ignoramos

                group.UserGroups.Add(new UserGroup { GroupId = request.GroupId, UserId = practitionerId });
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                CreationDate = group.CreationDate,
                Status = group.Status.ToString(),
                PractitionerIds = group.UserGroups.Select(ug => ug.UserId).ToList()
            };
        }
    }
}