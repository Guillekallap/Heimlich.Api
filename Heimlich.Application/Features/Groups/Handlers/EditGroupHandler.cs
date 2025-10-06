using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Commands;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Groups.Handlers
{
    public class EditGroupHandler : IRequestHandler<EditGroupCommand, GroupDto>
    {
        private readonly HeimlichDbContext _context;

        public EditGroupHandler(HeimlichDbContext context)
        { _context = context; }

        public async Task<GroupDto> Handle(EditGroupCommand request, CancellationToken cancellationToken)
        {
            var group = await _context.Groups.Include(g => g.UserGroups).FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);
            if (group == null) throw new InvalidOperationException("Grupo no encontrado");
            group.Name = request.Name ?? group.Name;
            group.Description = request.Description ?? group.Description;
            // Actualizar practicantes: eliminar los que no estén y agregar nuevos
            var newSet = new HashSet<string>(request.PractitionerIds ?? new List<string>());
            var existingSet = new HashSet<string>(group.UserGroups.Select(ug => ug.UserId));
            // Remove
            var toRemove = group.UserGroups.Where(ug => !newSet.Contains(ug.UserId)).ToList();
            foreach (var rem in toRemove) group.UserGroups.Remove(rem);
            // Add
            var toAdd = newSet.Where(id => !existingSet.Contains(id));
            foreach (var addId in toAdd)
            {
                if (await _context.Users.AnyAsync(u => u.Id == addId, cancellationToken))
                    group.UserGroups.Add(new Domain.Entities.UserGroup { GroupId = group.Id, UserId = addId });
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