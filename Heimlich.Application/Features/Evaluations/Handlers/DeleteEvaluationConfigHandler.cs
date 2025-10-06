using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Evaluations.Handlers
{
    public class DeleteEvaluationConfigHandler : IRequestHandler<DeleteEvaluationConfigCommand, bool>
    {
        private readonly HeimlichDbContext _context;

        public DeleteEvaluationConfigHandler(HeimlichDbContext context)
        { _context = context; }

        public async Task<bool> Handle(DeleteEvaluationConfigCommand request, CancellationToken cancellationToken)
        {
            var config = await _context.EvaluationConfigs.FirstOrDefaultAsync(c => c.GroupId == request.GroupId, cancellationToken);
            if (config == null) return false;
            if (config.IsDefault) throw new InvalidOperationException("No se puede eliminar la configuración default.");
            _context.EvaluationConfigs.Remove(config);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}