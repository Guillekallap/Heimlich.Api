using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Queries;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class GetEvaluationsForInstructorHandler : IRequestHandler<GetEvaluationsForInstructorQuery, IEnumerable<EvaluationDto>>
    {
        private readonly HeimlichDbContext _context;
        public GetEvaluationsForInstructorHandler(HeimlichDbContext context) { _context = context; }

        public async Task<IEnumerable<EvaluationDto>> Handle(GetEvaluationsForInstructorQuery request, CancellationToken cancellationToken)
        {
            var evals = await _context.Evaluations
                .Where(e => e.EvaluatorId == request.InstructorId)
                .OrderByDescending(e => e.CreationDate)
                .ToListAsync(cancellationToken);
            return evals.Select(e => new EvaluationDto
            {
                Id = e.Id,
                EvaluatorId = e.EvaluatorId,
                EvaluatedUserId = e.EvaluatedUserId,
                Score = e.Score,
                Comments = e.Comments,
                IsValid = e.IsValid,
                State = e.State
            });
        }
    }
}
