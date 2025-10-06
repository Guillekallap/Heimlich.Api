using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Commands;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Groups.Handlers
{
    public class ResetEvaluationParametersHandler : IRequestHandler<ResetEvaluationParametersCommand, GroupDto>
    {
        private readonly HeimlichDbContext _context;

        public ResetEvaluationParametersHandler(HeimlichDbContext context)
        {
            _context = context;
        }

        public async Task<GroupDto> Handle(ResetEvaluationParametersCommand request, CancellationToken cancellationToken)
        {
            var group = await _context.Groups
                .Include(g => g.UserGroups)
                .FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);

            if (group == null)
                throw new InvalidOperationException("El grupo no existe.");

            // Buscar configuración default (IsDefault = true) y asignarla / crear si no existe
            var defaultConfig = await _context.EvaluationConfigs.FirstOrDefaultAsync(c => c.IsDefault, cancellationToken);
            if (defaultConfig == null)
            {
                defaultConfig = new Domain.Entities.EvaluationConfig
                {
                    GroupId = group.Id,
                    MaxErrors = 10,
                    MaxTime = 30,
                    IsDefault = true
                };
                _context.EvaluationConfigs.Add(defaultConfig);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else if (defaultConfig.GroupId != group.Id)
            {
                // Clonar la config default global para este grupo
                var cloned = new Domain.Entities.EvaluationConfig
                {
                    GroupId = group.Id,
                    MaxErrors = defaultConfig.MaxErrors,
                    MaxTime = defaultConfig.MaxTime,
                    IsDefault = false
                };
                _context.EvaluationConfigs.Add(cloned);
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