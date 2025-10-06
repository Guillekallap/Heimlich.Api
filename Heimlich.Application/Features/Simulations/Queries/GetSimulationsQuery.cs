using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Simulations.Queries
{
    public class GetSimulationsQuery : IRequest<IEnumerable<SimulationSessionDto>>
    {
        public string PractitionerId { get; }

        public GetSimulationsQuery(string practitionerId)
        {
            PractitionerId = practitionerId;
        }
    }
}