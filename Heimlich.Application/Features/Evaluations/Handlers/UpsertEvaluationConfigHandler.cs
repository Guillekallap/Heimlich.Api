using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class UpsertEvaluationConfigHandler : IRequestHandler<UpsertEvaluationConfigCommand, EvaluationConfig>
    {
        private readonly HeimlichDbContext _context;

        public UpsertEvaluationConfigHandler(HeimlichDbContext context)
        { _context = context; }

        public async Task<EvaluationConfig> Handle(UpsertEvaluationConfigCommand request, CancellationToken cancellationToken)
        {
            // Buscar config por nombre (único)
            var existing = await _context.EvaluationConfigs.FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);
            if (existing != null)
            {
                // Solo vincular al grupo
                var currentLink = await _context.EvaluationConfigGroups.FirstOrDefaultAsync(l => l.GroupId == request.GroupId, cancellationToken);
                if (currentLink != null && currentLink.EvaluationConfigId != existing.Id)
                {
                    _context.EvaluationConfigGroups.Remove(currentLink);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                if (currentLink == null || currentLink.EvaluationConfigId != existing.Id)
                {
                    _context.EvaluationConfigGroups.Add(new EvaluationConfigGroup { GroupId = request.GroupId, EvaluationConfigId = existing.Id });
                    await _context.SaveChangesAsync(cancellationToken);
                }
                return existing;
            }

            var config = new EvaluationConfig
            {
                Name = request.Name,
                MaxErrors = request.MaxErrors,
                MaxSuccess = request.MaxSuccess,
                MaxTime = request.MaxTime,
                IsDefault = request.IsDefault,
                GroupId = null
            };
            _context.EvaluationConfigs.Add(config);
            await _context.SaveChangesAsync(cancellationToken);

            var linkOld = await _context.EvaluationConfigGroups.FirstOrDefaultAsync(l => l.GroupId == request.GroupId, cancellationToken);
            if (linkOld != null)
            {
                _context.EvaluationConfigGroups.Remove(linkOld);
                await _context.SaveChangesAsync(cancellationToken);
            }
            _context.EvaluationConfigGroups.Add(new EvaluationConfigGroup { GroupId = request.GroupId, EvaluationConfigId = config.Id });
            await _context.SaveChangesAsync(cancellationToken);
            return config;
        }
    }
}