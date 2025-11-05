using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Queries;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class GetPractitionerEvaluationsByGroupHandler : IRequestHandler<GetPractitionerEvaluationsByGroupQuery, List<GroupEvaluationsDto>>
    {
        private readonly HeimlichDbContext _context;

        public GetPractitionerEvaluationsByGroupHandler(HeimlichDbContext context)
        {
            _context = context;
        }

        public async Task<List<GroupEvaluationsDto>> Handle(GetPractitionerEvaluationsByGroupQuery request, CancellationToken cancellationToken)
        {
            // Obtener todas las evaluaciones validadas del practicante
            var evaluations = await _context.Evaluations
                .Include(e => e.Measurements)
                .Include(e => e.EvaluatedUser)
                .Include(e => e.Group)
                .Where(e => e.EvaluatedUserId == request.PractitionerId && e.State == SessionStateEnum.Validated)
                .OrderByDescending(e => e.CreationDate)
                .ToListAsync(cancellationToken);

            // Agrupar por GroupId
            var groupedEvaluations = evaluations
                .GroupBy(e => e.GroupId)
                .Select(g => new GroupEvaluationsDto
                {
                    GroupId = g.Key,
                    GroupName = g.First().Group?.Name,
                    Evaluations = g.Select(e => new EvaluationDto
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
                            Result = m.Status ? "CORRECT" : "INCORRECT",
                            AngleDeg = m.AngleDeg ?? string.Empty,
                            AngleStatus = m.AngleStatus,
                            ForceValue = m.ForceValue ?? string.Empty,
                            ForceStatus = m.ForceStatus,
                            TouchStatus = m.TouchStatus,
                            Status = m.Status,
                            Message = m.Message,
                            IsValid = m.IsValid
                        }).ToList()
                    }).ToList()
                })
                .OrderBy(g => g.GroupName)
                .ToList();

            return groupedEvaluations;
        }
    }
}
