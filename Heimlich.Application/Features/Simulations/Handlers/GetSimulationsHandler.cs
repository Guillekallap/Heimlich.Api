using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Simulations.Queries;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Heimlich.Application.Features.Simulations.Handlers
{
    public class GetSimulationsHandler : IRequestHandler<GetSimulationsQuery, IEnumerable<SimulationSessionDto>>
    {
        private readonly HeimlichDbContext _context;
        private readonly IMapper _mapper;

        public GetSimulationsHandler(HeimlichDbContext context, IMapper mapper)
        { _context = context; _mapper = mapper; }

        public async Task<IEnumerable<SimulationSessionDto>> Handle(GetSimulationsQuery request, CancellationToken cancellationToken)
        {
            var sims = await _context.Simulations
                .Where(s => s.PractitionerId == request.PractitionerId && s.State != SessionStateEnum.Cancelled)
                .Include(s => s.Measurements)
                .OrderByDescending(s => s.CreationDate)
                .ToListAsync(cancellationToken);

            var result = sims.Select(sim => _mapper.Map<SimulationSessionDto>(sim)).ToList();

            return result;
        }
    }
}