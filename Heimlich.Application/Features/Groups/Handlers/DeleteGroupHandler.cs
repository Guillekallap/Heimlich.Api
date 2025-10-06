using Heimlich.Application.Features.Groups.Commands;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Groups.Handlers
{
    public class DeleteGroupHandler : IRequestHandler<DeleteGroupCommand, Unit>
    {
        private readonly HeimlichDbContext _context;

        public DeleteGroupHandler(HeimlichDbContext context)
        { _context = context; }

        public async Task<Unit> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);
            if (group == null)
                return Unit.Value; // Silencioso si no existe
            if (group.Status != GroupStatusEnum.Inactive)
            {
                group.Status = GroupStatusEnum.Inactive;
                await _context.SaveChangesAsync(cancellationToken);
            }
            return Unit.Value;
        }
    }
}