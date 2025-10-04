using MediatR;
using Heimlich.Application.DTOs;

namespace Heimlich.Application.Features.Simulations.Commands
{
    public class CancelSimulationCommand : IRequest<SimulationSessionDto>
    {
        public int SimulationId { get; }
        public string PractitionerId { get; }
        public CancelSimulationCommand(int simulationId, string practitionerId)
        {
            SimulationId = simulationId;
            PractitionerId = practitionerId;
        }
    }
}
