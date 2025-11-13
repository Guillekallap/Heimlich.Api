using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Queries;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Groups.Handlers
{
    public class GetOwnedGroupsHandler : IRequestHandler<GetOwnedGroupsQuery, List<GroupDto>>
    {
        private readonly HeimlichDbContext _context;

        public GetOwnedGroupsHandler(HeimlichDbContext context)
        { _context = context; }

        public async Task<List<GroupDto>> Handle(GetOwnedGroupsQuery request, CancellationToken cancellationToken)
        {
            var groups = await _context.Groups
                .Where(g => g.OwnerInstructorId == request.InstructorId && g.Status == GroupStatusEnum.Active)
                .Include(g => g.UserGroups)
                .Include(g => g.EvaluationConfigGroups)
                    .ThenInclude(ecg => ecg.EvaluationConfig)
                .OrderByDescending(g => g.CreationDate)
                .ToListAsync(cancellationToken);

            var ownerName = await _context.Users
                .Where(u => u.Id == request.InstructorId)
                .Select(u => u.Fullname)
                .FirstOrDefaultAsync(cancellationToken);

            return groups.Select(g =>
            {
                // Get the evaluation config for this group (should be only one active config per group)
                var configGroup = g.EvaluationConfigGroups.FirstOrDefault();
                var config = configGroup?.EvaluationConfig;

                return new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    CreationDate = g.CreationDate,
                    EvaluationDate = g.EvaluationDate,
                    Status = g.Status.ToString(),
                    OwnerInstructorId = g.OwnerInstructorId,
                    OwnerInstructorName = ownerName,
                    PractitionerIds = g.UserGroups.Select(ug => ug.UserId).ToList(),
                    EvaluationConfig = config != null ? new EvaluationParametersDto
                    {
                        Id = config.Id,
                        Name = config.Name,
                        MaxErrors = config.MaxErrors,
                        MaxSuccess = config.MaxSuccess,
                        MaxTime = config.MaxTime,
                        MaxTimeInterval = config.MaxTimeInterval,
                        IsDefault = config.IsDefault,
                        Status = config.Status.ToString()
                    } : null
                };
            }).ToList();
        }
    }
}