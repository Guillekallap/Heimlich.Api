using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Queries;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class GetEvaluationsForInstructorHandler : IRequestHandler<GetEvaluationsForInstructorQuery, IEnumerable<EvaluationDto>>
    {
        private readonly HeimlichDbContext _context;

        public GetEvaluationsForInstructorHandler(HeimlichDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EvaluationDto>> Handle(GetEvaluationsForInstructorQuery request, CancellationToken cancellationToken)
        {
            var evaluations = await _context.Evaluations
                .Include(e => e.Measurements)
                .Where(e => e.EvaluatorId == request.InstructorId)
                .ToListAsync(cancellationToken);

            var defaultConfig = await _context.EvaluationConfigs.FirstOrDefaultAsync(c => c.IsDefault, cancellationToken);
            int? defaultConfigId = defaultConfig?.Id;

            return evaluations.Select(e => new EvaluationDto
            {
                Id = e.Id,
                EvaluatorId = e.EvaluatorId,
                EvaluatedUserId = e.EvaluatedUserId,
                TrunkId = e.TrunkId,
                GroupId = e.GroupId,
                EvaluationConfigId = e.EvaluationConfigId ?? defaultConfigId,
                Score = e.Score,
                Comments = e.Comments,
                IsValid = e.IsValid,
                State = e.State,
                TotalErrors = e.TotalErrors,
                TotalSuccess = e.TotalSuccess,
                TotalMeasurements = e.TotalMeasurements,
                SuccessRate = e.SuccessRate,
                Measurements = e.Measurements.OrderBy(m => m.ElapsedMs).Select(m => new EvaluationMeasurementDto
                {
                    ElapsedMs = m.ElapsedMs,
                    ForceValue = m.ForceValue ?? string.Empty,
                    ForceIsValid = m.ForceStatus,
                    TouchValue = m.TouchStatus ? "true" : "false",
                    TouchIsValid = m.TouchStatus,
                    HandPositionValue = m.AngleDeg ?? string.Empty,
                    HandPositionIsValid = m.AngleStatus,
                    PositionValue = m.Message,
                    PositionIsValid = m.Status,
                    IsValid = m.IsValid
                }).ToList()
            });
        }
    }
}