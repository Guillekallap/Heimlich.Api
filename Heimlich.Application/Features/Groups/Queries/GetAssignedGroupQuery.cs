using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Groups.Queries
{
    public class GetAssignedGroupQuery : IRequest<GroupDto>
    {
        public string UserId { get; }

        public GetAssignedGroupQuery(string userId)
        {
            UserId = userId;
        }
    }
}