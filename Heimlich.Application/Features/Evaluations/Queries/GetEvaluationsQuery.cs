using Heimlich.Application.DTOs;
using MediatR;

public class GetEvaluationsQuery : IRequest<List<EvaluationDto>>
{
    public int? GroupId { get; }
    public string? EvaluatedUserId { get; }

    public GetEvaluationsQuery(int? groupId, string? evaluatedUserId = null)
    {
        GroupId = groupId;
        EvaluatedUserId = evaluatedUserId;
    }
}