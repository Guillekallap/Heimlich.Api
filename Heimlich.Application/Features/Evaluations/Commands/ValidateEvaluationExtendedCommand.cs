using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Evaluations.Commands
{
    public class ValidateEvaluationExtendedCommand : IRequest<EvaluationDto>
    {
        public int EvaluationId { get; }
        public string EvaluatorId { get; }
        public double Score { get; }
        public bool IsValid { get; }
        public string? Comments { get; }
        public string Signature { get; }
        public int EvaluationConfigId { get; }

        public ValidateEvaluationExtendedCommand(int evaluationId, string evaluatorId, double score, bool isValid, string? comments, string signature, int evaluationConfigId)
        {
            EvaluationId = evaluationId;
            EvaluatorId = evaluatorId;
            Score = score;
            IsValid = isValid;
            Comments = comments;
            Signature = signature;
            EvaluationConfigId = evaluationConfigId;
        }
    }
}