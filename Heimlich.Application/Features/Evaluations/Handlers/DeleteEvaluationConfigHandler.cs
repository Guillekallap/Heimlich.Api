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
            var link = await _context.EvaluationConfigGroups
                .FirstOrDefaultAsync(l => l.GroupId == request.GroupId, cancellationToken);
            if (link == null) return false;
            _context.EvaluationConfigGroups.Remove(link);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}