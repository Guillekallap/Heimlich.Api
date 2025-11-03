using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class ValidateEvaluationExtendedHandler : IRequestHandler<ValidateEvaluationExtendedCommand, EvaluationDto>
    {
        private readonly HeimlichDbContext _context;
        private readonly IMapper _mapper;

        public ValidateEvaluationExtendedHandler(HeimlichDbContext context, IMapper mapper)
        { 
            _context = context;
            _mapper = mapper;
        }

        public async Task<EvaluationDto> Handle(ValidateEvaluationExtendedCommand request, CancellationToken cancellationToken)
        {
            var evaluation = await _context.Evaluations
                .Include(e => e.Measurements)
                .Include(e => e.EvaluatedUser)
                .FirstOrDefaultAsync(e => e.Id == request.EvaluationId, cancellationToken);
            if (evaluation == null) throw new KeyNotFoundException("Evaluación no encontrada");
            if (evaluation.EvaluatedUserId == null)
                throw new InvalidOperationException("No se puede validar una evaluación sin practicante asignado.");
            if (evaluation.State == SessionStateEnum.Validated)
                throw new InvalidOperationException("La evaluación ya fue validada.");
            if (evaluation.State == SessionStateEnum.Cancelled)
                throw new InvalidOperationException("No se puede validar una evaluación cancelada.");
            var config = await _context.EvaluationConfigs.FirstOrDefaultAsync(c => c.Id == request.EvaluationConfigId, cancellationToken);
            if (config == null)
                throw new KeyNotFoundException("Configuración de evaluación no encontrada.");
            evaluation.Score = request.Score;
            evaluation.Comments = request.Comments;
            evaluation.Signature = request.Signature;
            evaluation.EvaluationConfigId = config.Id;
            evaluation.State = SessionStateEnum.Validated;
            evaluation.ValidatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            
            return _mapper.Map<EvaluationDto>(evaluation);
        }
    }
}