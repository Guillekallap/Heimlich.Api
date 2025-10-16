using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Queries;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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
                .Where(e => e.EvaluatorId == request.InstructorId)
                .ToListAsync(cancellationToken);

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
                State = e.State
            });
        }
    }
}