using Heimlich.Application.DTOs;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class GetEvaluationsHandler : IRequestHandler<GetEvaluationsQuery, List<EvaluationDto>>
    {
        private readonly HeimlichDbContext _context;
        private readonly IMapper _mapper;

        public GetEvaluationsHandler(HeimlichDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

            // Map using AutoMapper (measurements are already included so Ids will be available)
            var result = _mapper.Map<List<EvaluationDto>>(evaluations);

            return result;
        }
    }
}