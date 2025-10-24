using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Queries;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper;
using System.Collections.Generic;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class GetEvaluationsForInstructorHandler : IRequestHandler<GetEvaluationsForInstructorQuery, IEnumerable<EvaluationDto>>
    {
        private readonly HeimlichDbContext _context;
        private readonly IMapper _mapper;

        public GetEvaluationsForInstructorHandler(HeimlichDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EvaluationDto>> Handle(GetEvaluationsForInstructorQuery request, CancellationToken cancellationToken)
        {
            var evaluations = await _context.Evaluations
                .Include(e => e.Measurements)
                .Where(e => e.EvaluatorId == request.InstructorId)
                .ToListAsync(cancellationToken);

            var result = _mapper.Map<IEnumerable<EvaluationDto>>(evaluations);

            return result;
        }
    }
}