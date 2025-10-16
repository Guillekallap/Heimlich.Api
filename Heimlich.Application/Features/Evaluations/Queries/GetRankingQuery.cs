using Heimlich.Application.DTOs;
using MediatR;
using System.Collections.Generic;

public class GetRankingQuery : IRequest<List<PractitionerRankingDto>>
{
    public int? GroupId { get; }

    public GetRankingQuery(int? groupId)
    {
        GroupId = groupId;
    }
}