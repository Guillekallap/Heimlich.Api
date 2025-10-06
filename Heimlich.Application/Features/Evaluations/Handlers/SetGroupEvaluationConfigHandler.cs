using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class SetGroupEvaluationConfigHandler : IRequestHandler<SetGroupEvaluationConfigCommand, EvaluationConfig>
    {
        private readonly HeimlichDbContext _context;

        public SetGroupEvaluationConfigHandler(HeimlichDbContext context)
        { _context = context; }

        public async Task<EvaluationConfig> Handle(SetGroupEvaluationConfigCommand request, CancellationToken cancellationToken)
        {
            var group = await _context.Groups.Include(g => g.EvaluationConfigGroups).FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);
            if (group == null) throw new KeyNotFoundException("Grupo no encontrado");
            var config = await _context.EvaluationConfigs.FirstOrDefaultAsync(c => c.Id == request.EvaluationConfigId, cancellationToken);
            if (config == null) throw new KeyNotFoundException("Configuración no encontrada");

            // Si ya está asociada, no hacer nada
            if (group.EvaluationConfigGroups.Any(ecg => ecg.EvaluationConfigId == config.Id))
                return config;

            // Eliminar asociación previa (solo una activa por grupo)
            var existing = group.EvaluationConfigGroups.ToList();
            if (existing.Any())
            {
                _context.EvaluationConfigGroups.RemoveRange(existing);
            }
            // Agregar nueva asociación
            var link = new EvaluationConfigGroup { GroupId = group.Id, EvaluationConfigId = config.Id };
            _context.EvaluationConfigGroups.Add(link);
            await _context.SaveChangesAsync(cancellationToken);
            return config;
        }
    }
}