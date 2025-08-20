using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Evaluations.Commands
{
    public class CreateEvaluationCommand : IRequest<EvaluationDto>
    {
        public int PracticeSessionId { get; set; }
        public string EvaluatorId { get; set; }
        public string EvaluatedUserId { get; set; }
        public double Score { get; set; }
        public string Comments { get; set; }
        public List<MeasurementDto> Measurements { get; set; }
    }
}