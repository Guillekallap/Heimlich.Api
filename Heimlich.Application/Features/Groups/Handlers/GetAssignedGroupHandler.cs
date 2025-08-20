using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Queries;
using MediatR;

namespace Heimlich.Application.Features.Groups.Handlers
{
    public class GetAssignedGroupHandler : IRequestHandler<GetAssignedGroupQuery, GroupDto>
    {
        // Inyección de dependencias

        public async Task<GroupDto> Handle(GetAssignedGroupQuery request, CancellationToken cancellationToken)
        {
            // Lógica para buscar el grupo asignado al usuario
        }
    }
}