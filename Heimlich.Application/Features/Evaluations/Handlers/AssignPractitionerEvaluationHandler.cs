using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class AssignPractitionerEvaluationHandler : IRequestHandler<AssignPractitionerEvaluationCommand, EvaluationDto>
    {
        private readonly HeimlichDbContext _context;
        private readonly IMapper _mapper;

        public AssignPractitionerEvaluationHandler(HeimlichDbContext context, IMapper mapper)
        { 
            _context = context;
            _mapper = mapper;
        }

        public async Task<EvaluationDto> Handle(AssignPractitionerEvaluationCommand request, CancellationToken cancellationToken)
        {
            var eval = await _context.Evaluations
                .Include(e => e.Measurements)
                .Include(e => e.EvaluatedUser)
                .FirstOrDefaultAsync(e => e.Id == request.EvaluationId, cancellationToken);
            if (eval == null) throw new KeyNotFoundException("Evaluación no encontrada");
            if (!await _context.Users.AnyAsync(u => u.Id == request.NewPractitionerUserId, cancellationToken))
                throw new KeyNotFoundException("Usuario practicante no existe");
            if (eval.EvaluatedUserId == request.NewPractitionerUserId)
                return _mapper.Map<EvaluationDto>(eval); // ya asignado
            eval.EvaluatedUserId = request.NewPractitionerUserId;
            await _context.SaveChangesAsync(cancellationToken);
            
            // Reload to get the new user
            await _context.Entry(eval).Reference(e => e.EvaluatedUser).LoadAsync(cancellationToken);
            return _mapper.Map<EvaluationDto>(eval);
        }
    }
}