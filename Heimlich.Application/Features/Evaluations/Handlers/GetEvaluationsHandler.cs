using Heimlich.Application.DTOs;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class GetEvaluationsHandler : IRequestHandler<GetEvaluationsQuery, List<EvaluationDto>>
    {
        private readonly HeimlichDbContext _context;

        public GetEvaluationsHandler(HeimlichDbContext context)
        {
            _context = context;
        }

        public async Task<List<EvaluationDto>> Handle(GetEvaluationsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Evaluations.Include(e => e.Measurements).AsQueryable();

            if (request.GroupId.HasValue)
            {
                query = query.Where(e => e.GroupId == request.GroupId.Value);
            }
            if (!string.IsNullOrEmpty(request.EvaluatedUserId))
            {
                query = query.Where(e => e.EvaluatedUserId == request.EvaluatedUserId);
            }

            var evaluations = await query.ToListAsync(cancellationToken);

            return evaluations.Select(e => new EvaluationDto
            {
                Id = e.Id,
                EvaluatorId = e.EvaluatorId,
                EvaluatedUserId = e.EvaluatedUserId,
                TrunkId = e.TrunkId,
                GroupId = e.GroupId,
                EvaluationConfigId = e.EvaluationConfigId,
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
                    ForceValue = m.ForceValue,
                    ForceIsValid = m.ForceIsValid,
                    TouchValue = m.TouchValue,
                    TouchIsValid = m.TouchIsValid,
                    HandPositionValue = m.HandPositionValue,
                    HandPositionIsValid = m.HandPositionIsValid,
                    PositionValue = m.PositionValue,
                    PositionIsValid = m.PositionIsValid,
                    IsValid = m.IsValid
                }).ToList()
            }).ToList();
        }
    }
}