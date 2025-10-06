using Heimlich.Application.DTOs;
using MediatR;

public class ValidateEvaluationCommand : IRequest<EvaluationDto>
{
    public int EvaluationId { get; }
    public bool IsValid { get; }
    public string Comments { get; }

    public ValidateEvaluationCommand(int evaluationId, bool isValid, string comments)
    {
        EvaluationId = evaluationId;
        IsValid = isValid;
        Comments = comments;
    }
}