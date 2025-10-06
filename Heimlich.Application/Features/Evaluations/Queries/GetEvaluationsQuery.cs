using Heimlich.Application.DTOs;
using MediatR;

public class GetEvaluationsQuery : IRequest<List<EvaluationDto>>
{
    public int? GroupId { get; }

    public GetEvaluationsQuery(int? groupId)
    {
        GroupId = groupId;
    }
}