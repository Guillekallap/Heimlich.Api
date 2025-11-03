using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class UnassignPractitionerEvaluationHandler : IRequestHandler<UnassignPractitionerEvaluationCommand, EvaluationDto>
    {
        private readonly HeimlichDbContext _context;
        private readonly IMapper _mapper;

        public UnassignPractitionerEvaluationHandler(HeimlichDbContext context, IMapper mapper)
        { 
            _context = context;
            _mapper = mapper;
        }

        public async Task<EvaluationDto> Handle(UnassignPractitionerEvaluationCommand request, CancellationToken cancellationToken)
        {
            var eval = await _context.Evaluations
                .Include(e => e.Measurements)
                .Include(e => e.EvaluatedUser)
                .FirstOrDefaultAsync(e => e.Id == request.EvaluationId, cancellationToken);
            if (eval == null) throw new KeyNotFoundException("Evaluación no encontrada");
            if (eval.EvaluatedUserId == null)
                return _mapper.Map<EvaluationDto>(eval); // ya null
            eval.EvaluatedUserId = null; // asignar null coherente con nullable
            await _context.SaveChangesAsync(cancellationToken);
            return _mapper.Map<EvaluationDto>(eval);
        }
    }
}