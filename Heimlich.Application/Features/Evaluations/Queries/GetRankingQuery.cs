using Heimlich.Application.DTOs;
using MediatR;

public class GetRankingQuery : IRequest<List<PractitionerRankingDto>>
{
    public int? GroupId { get; }

    public GetRankingQuery(int? groupId)
    {
        GroupId = groupId;
    }
}