using Swashbuckle.AspNetCore.Filters;
using Heimlich.Application.DTOs;
using System.Collections.Generic;

namespace Heimlich.Api.SwaggerExamples
{
    public class CreateSimulationDtoExample : IExamplesProvider<CreateSimulationDto>
    {
        public CreateSimulationDto GetExamples()
        {
            return new CreateSimulationDto
            {
                TrunkId = 1,
                Comments = "Práctica de simulación automática",
                Measurements = new List<SimulationMeasurementDto>
                {
                    new SimulationMeasurementDto { ElapsedMs = 0, ForceValue = "10", ForceStatus = true, TouchStatus = true, AngleDeg = "7", AngleStatus = true, Status = true, IsValid = true },
                    new SimulationMeasurementDto { ElapsedMs = 1000, ForceValue = "11", ForceStatus = true, TouchStatus = true, AngleDeg = "7", AngleStatus = true, Status = true, IsValid = true }
                }
            };
        }
    }
}
