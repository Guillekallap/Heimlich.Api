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
            var config = await _context.EvaluationConfigs.FirstOrDefaultAsync(c => c.GroupId == request.GroupId, cancellationToken);
            if (config == null)
            {
                config = new EvaluationConfig
                {
                    GroupId = request.GroupId,
                    MaxErrors = 10,
                    MaxTime = 30,
                    Name = "Default",
                    IsDefault = true
                };
                _context.EvaluationConfigs.Add(config);
            }
            else
            {
                config.MaxErrors = 10;
                config.MaxTime = 30;
                config.Name = "Default";
                config.IsDefault = true;
            }
            await _context.SaveChangesAsync(cancellationToken);
            return config;
        }
    }
}