using MediatR;
using Heimlich.Application.DTOs;

public class GetEvaluationsQuery : IRequest<List<EvaluationDto>>
{
    public int? GroupId { get; }
    public GetEvaluationsQuery(int? groupId)
    {
        GroupId = groupId;
    }
}