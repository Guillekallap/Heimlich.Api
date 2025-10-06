using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Heimlich.Application.Features.Groups.Handlers
{
    public class CreateGroupHandler : IRequestHandler<CreateGroupCommand, GroupDto>
    {
        private readonly HeimlichDbContext _context;
        private readonly IHttpContextAccessor _http;

        public CreateGroupHandler(HeimlichDbContext context, IHttpContextAccessor http)
        { _context = context; _http = http; }

        private async Task<int> EnsureConfigLinkAsync(int groupId, int? evaluationConfigId, CancellationToken cancellationToken)
        {
            // Si viene un id de config, validar y vincular
            if (evaluationConfigId.HasValue)
            {
                var config = await _context.EvaluationConfigs.FirstOrDefaultAsync(c => c.Id == evaluationConfigId.Value, cancellationToken);
                if (config == null) throw new KeyNotFoundException("Configuración de evaluación no encontrada");
                _context.EvaluationConfigGroups.Add(new EvaluationConfigGroup { GroupId = groupId, EvaluationConfigId = config.Id });
                await _context.SaveChangesAsync(cancellationToken);
                return config.Id;
            }
            // Vincular default global
            var defaultConfig = await _context.EvaluationConfigs.FirstOrDefaultAsync(c => c.IsDefault, cancellationToken);
            if (defaultConfig == null)
            {
                defaultConfig = new EvaluationConfig { Name = "Default", MaxErrors = 10, MaxTime = 30, IsDefault = true };
                _context.EvaluationConfigs.Add(defaultConfig);
                await _context.SaveChangesAsync(cancellationToken);
            }
            _context.EvaluationConfigGroups.Add(new EvaluationConfigGroup { GroupId = groupId, EvaluationConfigId = defaultConfig.Id });
            await _context.SaveChangesAsync(cancellationToken);
            return defaultConfig.Id;
        }

        public async Task<GroupDto> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var ownerId = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entity = new Group
            {
                Name = request.Name.Trim(),
                Description = (request.Description ?? string.Empty).Trim(),
                CreationDate = DateTime.UtcNow,
                Status = GroupStatusEnum.Active,
                OwnerInstructorId = ownerId!
            };
            _context.Groups.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            // Practicantes
            if (request.PractitionerIds != null && request.PractitionerIds.Any())
            {
                foreach (var uid in request.PractitionerIds.Distinct())
                {
                    if (await _context.Users.AnyAsync(u => u.Id == uid, cancellationToken) &&
                        !await _context.UserGroups.AnyAsync(ug => ug.GroupId == entity.Id && ug.UserId == uid, cancellationToken))
                    {
                        _context.UserGroups.Add(new UserGroup { GroupId = entity.Id, UserId = uid });
                    }
                }
                await _context.SaveChangesAsync(cancellationToken);
            }

            // Vincular configuración (default o específica) - se obtiene desde DTO extendido: necesitamos EvaluationConfigId => agregarlo al command si no existe.
            if (request is CreateGroupCommandWithConfig extended)
            {
                await EnsureConfigLinkAsync(entity.Id, extended.EvaluationConfigId, cancellationToken);
            }
            else
            {
                await EnsureConfigLinkAsync(entity.Id, null, cancellationToken);
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
                CreationDate = entity.CreationDate,
                Status = entity.Status.ToString(),
                PractitionerIds = practitioners
            };
        }
    }

    // Command extendido para soportar EvaluationConfigId sin romper llamadas existentes (si existían)
    public class CreateGroupCommandWithConfig : CreateGroupCommand
    {
        public int? EvaluationConfigId { get; }
        public CreateGroupCommandWithConfig(string name, string? description, List<string>? practitionerIds, int? evaluationConfigId)
            : base(name, description, practitionerIds)
        {
            EvaluationConfigId = evaluationConfigId;
        }
    }
}