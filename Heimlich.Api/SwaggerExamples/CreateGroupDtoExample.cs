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
                Name = "Grupo RCP B�sico",
                Description = "Grupo para pr�cticas de RCP - nivel b�sico",
                PractitionerIds = new List<string> { "user1", "user2" },
                EvaluationConfigId = null // use default
            };
        }
    }
}
