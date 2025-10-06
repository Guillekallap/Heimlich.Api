using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Simulations.Commands
{
    public class CancelSimulationImmediateCommand : IRequest<SimulationSessionDto>
    {
        public CreateSimulationDto Dto { get; }
        public string PractitionerId { get; }

        public CancelSimulationImmediateCommand(CreateSimulationDto dto, string practitionerId)
        {
            Dto = dto;
            PractitionerId = practitionerId;
        }
    }
}