using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Evaluations.Commands
{
    public class CreateEvaluationCommand : IRequest<EvaluationDto>
    {
        public CreateEvaluationDto Dto { get; }
        public string EvaluatorId { get; }

        public CreateEvaluationCommand(CreateEvaluationDto dto, string evaluatorId)
        {
            Dto = dto;
            EvaluatorId = evaluatorId;
        }
    }
}