using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Groups.Queries
{
    public class GetAssignedGroupQuery : IRequest<List<GroupDto>>
    {
        public string UserId { get; }

        public GetAssignedGroupQuery(string userId)
        {
            UserId = userId;
        }
    }
}