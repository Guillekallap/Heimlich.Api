using Swashbuckle.AspNetCore.Filters;
using Heimlich.Application.DTOs;
using System.Collections.Generic;

namespace Heimlich.Api.SwaggerExamples
{
    public class CreateEvaluationDtoExample : IExamplesProvider<CreateEvaluationDto>
    {
        public CreateEvaluationDto GetExamples()
        {
            return new CreateEvaluationDto
            {
                EvaluatedUserId = "pract1",
                TrunkId = 1,
                GroupId = 2,
                Score = 85,
                Comments = "Evaluación automática",
                Measurements = new List<EvaluationMeasurementInputDto>
                {
                    new EvaluationMeasurementInputDto { ElapsedMs = 0, Result = "OK", AngleDeg = "7", AngleStatus = true, ForceValue = "10", ForceStatus = true, TouchStatus = true, Status = true, Message = "pos", IsValid = true }
                }
            };
        }
    }
}
