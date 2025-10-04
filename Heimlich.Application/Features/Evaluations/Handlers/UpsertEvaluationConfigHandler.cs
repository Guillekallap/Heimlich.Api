using System.Threading;
using System.Threading.Tasks;
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
        public UpsertEvaluationConfigHandler(HeimlichDbContext context) { _context = context; }

        public async Task<EvaluationConfig> Handle(UpsertEvaluationConfigCommand request, CancellationToken cancellationToken)
        {
            var config = await _context.EvaluationConfigs.FirstOrDefaultAsync(c => c.GroupId == request.GroupId, cancellationToken);
            if (config == null)
            {
                config = new EvaluationConfig
                {
                    GroupId = request.GroupId,
                    MaxErrors = request.MaxErrors,
                    MaxTime = request.MaxTime,
                    IsDefault = request.IsDefault
                };
                _context.EvaluationConfigs.Add(config);
            }
            else
            {
                config.MaxErrors = request.MaxErrors;
                config.MaxTime = request.MaxTime;
                config.IsDefault = request.IsDefault;
            }
            await _context.SaveChangesAsync(cancellationToken);
            return config;
        }
    }
}
