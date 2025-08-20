using MediatR;
using Heimlich.Application.DTOs;

public class GetRankingQuery : IRequest<List<RankingDto>>
{
    public int? GroupId { get; }
    public GetRankingQuery(int? groupId)
    {
        GroupId = groupId;
    }
}