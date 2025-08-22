using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Commands;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Groups.Handlers
{
    public class UpdateEvaluationParametersHandler : IRequestHandler<UpdateEvaluationParametersCommand, GroupDto>
    {
        private readonly HeimlichDbContext _context;

        public UpdateEvaluationParametersHandler(HeimlichDbContext context)
        {
            _context = context;
        }

        public async Task<GroupDto> Handle(UpdateEvaluationParametersCommand request, CancellationToken cancellationToken)
        {
            var group = await _context.Groups
                .Include(g => g.PracticeSessions)
                .FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);

            if (group == null)
                throw new InvalidOperationException("El grupo no existe.");

            // Aqu� deber�as tener una entidad/configuraci�n para los par�metros de evaluaci�n
            // Ejemplo: Actualizar los par�metros en la entidad correspondiente
            // group.EvaluationParameters.SensorIntervals = request.SensorIntervals;
            // group.EvaluationParameters.MaxErrors = request.MaxErrors;
            // group.EvaluationParameters.MaxTime = request.MaxTime;
            // await _context.SaveChangesAsync(cancellationToken);

            // Simulaci�n de guardado
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