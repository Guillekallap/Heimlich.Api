using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            if (evaluationConfigId.HasValue)
            {
                var config = await _context.EvaluationConfigs.FirstOrDefaultAsync(c => c.Id == evaluationConfigId.Value, cancellationToken);
                if (config == null) throw new KeyNotFoundException("Configuración de evaluación no encontrada");
                _context.EvaluationConfigGroups.Add(new EvaluationConfigGroup { GroupId = groupId, EvaluationConfigId = config.Id });
                await _context.SaveChangesAsync(cancellationToken);
                return config.Id;
            }
            var defaultConfig = await _context.EvaluationConfigs.FirstOrDefaultAsync(c => c.IsDefault, cancellationToken);
            if (defaultConfig == null)
            {
                defaultConfig = new EvaluationConfig { Name = "Default", MaxErrors = 10, MaxSuccess = 30, MaxTime = 30, IsDefault = true };
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
            var ownerName = ownerId != null ? await _context.Users.Where(u => u.Id == ownerId).Select(u => u.Fullname).FirstOrDefaultAsync(cancellationToken) : null;
            var entity = new Group
            {
                Name = request.Name.Trim(),
                Description = (request.Description ?? string.Empty).Trim(),
                CreationDate = DateTime.UtcNow,
                EvaluationDate = request.EvaluationDate,
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

            await EnsureConfigLinkAsync(entity.Id, request.EvaluationConfigId, cancellationToken);

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
                EvaluationDate = entity.EvaluationDate,
                Status = entity.Status.ToString(),
                OwnerInstructorId = ownerId,
                OwnerInstructorName = ownerName,
                PractitionerIds = practitioners
            };
        }
    }
}