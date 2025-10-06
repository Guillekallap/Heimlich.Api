using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class ResetEvaluationConfigHandler : IRequestHandler<ResetEvaluationConfigCommand, EvaluationConfig>
    {
        private readonly HeimlichDbContext _context;

        public ResetEvaluationConfigHandler(HeimlichDbContext context)
        { _context = context; }

        public async Task<EvaluationConfig> Handle(ResetEvaluationConfigCommand request, CancellationToken cancellationToken)
        {
            var defaultConfig = await _context.EvaluationConfigs.FirstOrDefaultAsync(c => c.IsDefault, cancellationToken);
            if (defaultConfig == null)
            {
                defaultConfig = new EvaluationConfig
                {
                    Name = "Default",
                    MaxErrors = 10,
                    MaxTime = 30,
                    IsDefault = true,
                    GroupId = null
                };
                _context.EvaluationConfigs.Add(defaultConfig);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var currentLink = await _context.EvaluationConfigGroups.FirstOrDefaultAsync(l => l.GroupId == request.GroupId, cancellationToken);
            if (currentLink != null && currentLink.EvaluationConfigId != defaultConfig.Id)
            {
                _context.EvaluationConfigGroups.Remove(currentLink);
                await _context.SaveChangesAsync(cancellationToken);
            }
            if (currentLink == null || currentLink.EvaluationConfigId != defaultConfig.Id)
            {
                _context.EvaluationConfigGroups.Add(new EvaluationConfigGroup { GroupId = request.GroupId, EvaluationConfigId = defaultConfig.Id });
                await _context.SaveChangesAsync(cancellationToken);
            }
            return defaultConfig;
        }
    }
}