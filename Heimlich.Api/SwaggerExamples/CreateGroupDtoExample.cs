using Swashbuckle.AspNetCore.Filters;
using Heimlich.Application.DTOs;
using System.Collections.Generic;

namespace Heimlich.Api.SwaggerExamples
{
    public class CreateGroupDtoExample : IExamplesProvider<CreateGroupDto>
    {
        public CreateGroupDto GetExamples()
        {
            return new CreateGroupDto
            {
                Name = "Grupo RCP Básico",
                Description = "Grupo para prácticas de RCP - nivel básico",
                PractitionerIds = new List<string> { "user1", "user2" },
                EvaluationConfigId = null // use default
            };
        }
    }
}
