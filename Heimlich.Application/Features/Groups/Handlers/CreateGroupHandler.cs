using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Groups.Handlers
{
    public class CreateGroupHandler : IRequestHandler<CreateGroupCommand, GroupDto>
    {
        private readonly HeimlichDbContext _context;

        public CreateGroupHandler(HeimlichDbContext context) => _context = context;

        public async Task<GroupDto> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var entity = new Group
            {
                Name = request.Name,
                Description = request.Description ?? string.Empty,
                CreationDate = DateTime.UtcNow,
                Status = "Active"
            };
            _context.Groups.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            if (request.PractitionerIds != null && request.PractitionerIds.Any())
            {
                foreach (var uid in request.PractitionerIds.Distinct())
                {
                    if (await _context.Users.AnyAsync(u => u.Id == uid, cancellationToken))
                    {
                        _context.UserGroups.Add(new UserGroup { GroupId = entity.Id, UserId = uid });
                    }
                }
                await _context.SaveChangesAsync(cancellationToken);
            }

            var practitioners = await _context.UserGroups
                .Where(ug => ug.GroupId == entity.Id)
                .Select(ug => ug.UserId)
                .ToListAsync(cancellationToken);

            return new GroupDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                PractitionerIds = practitioners
            };
        }
    }
}