using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Queries;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Groups.Handlers
{
    public class GetAssignedGroupHandler : IRequestHandler<GetAssignedGroupQuery, List<GroupDto>>
    {
        private readonly HeimlichDbContext _context;

        public GetAssignedGroupHandler(HeimlichDbContext context)
        {
            _context = context;
        }

        public async Task<List<GroupDto>> Handle(GetAssignedGroupQuery request, CancellationToken cancellationToken)
        {
            var groups = await _context.UserGroups
                .Where(ug => ug.UserId == request.UserId)
                .Select(ug => ug.Group)
                .Distinct()
                .ToListAsync(cancellationToken);

            var groupIds = groups.Select(g => g.Id).ToList();
            var userGroupsByGroup = await _context.UserGroups
                .Where(ug => groupIds.Contains(ug.GroupId))
                .GroupBy(ug => ug.GroupId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.UserId).ToList(), cancellationToken);

            // Obtener nombres de owner
            var ownerIds = groups.Select(g => g.OwnerInstructorId).Where(id => id != null).Distinct().ToList();
            var owners = await _context.Users
                .Where(u => ownerIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Fullname, cancellationToken);

            // If userId provided, preload evaluations for that user grouped by groupId
            Dictionary<int, List<EvaluationDto>> evalsByGroup = new();
            if (!string.IsNullOrEmpty(request.UserId))
            {
                var evalList = await _context.Evaluations
                    .Where(e => e.EvaluatedUserId == request.UserId && e.GroupId != null)
                    .Include(e => e.Measurements)
                    .Include(e => e.EvaluatedUser)
                    .ToListAsync(cancellationToken);

                evalsByGroup = evalList
                    .GroupBy(e => e.GroupId!.Value)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => new EvaluationDto
                        {
                            Id = e.Id,
                            EvaluatorId = e.EvaluatorId,
                            EvaluatedUserId = e.EvaluatedUserId,
                            EvaluatedUserFullName = e.EvaluatedUser?.Fullname,
                            TrunkId = e.TrunkId,
                            GroupId = e.GroupId,
                            EvaluationConfigId = e.EvaluationConfigId,
                            Score = e.Score,
                            Comments = e.Comments,
                            State = e.State,
                            CreationDate = e.CreationDate,
                            ValidatedAt = e.ValidatedAt,
                            TotalErrors = e.TotalErrors,
                            TotalSuccess = e.TotalSuccess,
                            TotalMeasurements = e.TotalMeasurements,
                            SuccessRate = e.SuccessRate,
                            TotalDurationMs = e.TotalDurationMs,
                            AverageErrorsPerMeasurement = e.AverageErrorsPerMeasurement,
                            Measurements = e.Measurements.Select(m => new EvaluationMeasurementDto
                            {
                                Id = m.Id,
                                Timestamp = new DateTimeOffset(m.Time).ToUnixTimeMilliseconds(),
                                ElapsedMs = m.ElapsedMs ?? 0L,
                                Result = m.IsValid ? "CORRECT" : "INCORRECT",
                                AngleDeg = m.AngleDeg ?? string.Empty,
                                AngleStatus = m.AngleStatus,
                                ForceValue = m.ForceValue ?? string.Empty,
                                ForceStatus = m.ForceStatus,
                                TouchStatus = m.TouchStatus,
                                Status = m.Status,
                                Message = m.Message,
                                IsValid = m.IsValid
                            }).ToList()
                        }).ToList());
            }

            return groups.Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                CreationDate = g.CreationDate,
                EvaluationDate = g.EvaluationDate,
                Status = g.Status.ToString(),
                OwnerInstructorId = g.OwnerInstructorId,
                OwnerInstructorName = g.OwnerInstructorId != null && owners.ContainsKey(g.OwnerInstructorId) ? owners[g.OwnerInstructorId] : null,
                PractitionerIds = userGroupsByGroup.ContainsKey(g.Id) ? userGroupsByGroup[g.Id] : new List<string>()
            }).ToList();
        }
    }
}