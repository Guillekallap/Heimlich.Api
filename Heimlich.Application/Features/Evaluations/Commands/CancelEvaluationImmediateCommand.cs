using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Evaluations.Commands
{
    public class CancelEvaluationImmediateCommand : IRequest<EvaluationDto>
    {
        public CreateEvaluationDto Dto { get; }
        public string EvaluatorId { get; }

        public CancelEvaluationImmediateCommand(CreateEvaluationDto dto, string evaluatorId)
        {
            Dto = dto;
            EvaluatorId = evaluatorId;
        }
    }
}