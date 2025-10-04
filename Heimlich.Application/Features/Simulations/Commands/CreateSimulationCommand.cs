using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Simulations.Commands
{
    public class CreateSimulationCommand : IRequest<SimulationSessionDto>
    {
        public CreateSimulationDto Dto { get; }
        public string PractitionerId { get; }
        public CreateSimulationCommand(CreateSimulationDto dto, string practitionerId)
        {
            Dto = dto;
            PractitionerId = practitionerId;
        }
    }
}
