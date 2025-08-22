using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Commands;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Groups.Handlers
{
    public class ResetEvaluationParametersHandler : IRequestHandler<ResetEvaluationParametersCommand, GroupDto>
    {
        private readonly HeimlichDbContext _context;

        public ResetEvaluationParametersHandler(HeimlichDbContext context)
        {
            _context = context;
        }

        public async Task<GroupDto> Handle(ResetEvaluationParametersCommand request, CancellationToken cancellationToken)
        {
            var group = await _context.Groups
                .Include(g => g.PracticeSessions)
                .FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);

            if (group == null)
                throw new InvalidOperationException("El grupo no existe.");

            // Restablecer valores predeterminados
            // group.EvaluationParameters = new EvaluationParameters { ... };
            // await _context.SaveChangesAsync(cancellationToken);

            // Simulación de guardado
            await Task.CompletedTask;

            return new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                PractitionerIds = group.UserGroups.Select(ug => ug.UserId).ToList()
            };
        }
    }
}