using MediatR;
using Heimlich.Application.DTOs;

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