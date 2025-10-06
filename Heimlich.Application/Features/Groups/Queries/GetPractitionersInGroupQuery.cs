using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Groups.Queries
{
    public class GetPractitionersInGroupQuery : IRequest<List<UserDto>>
    {
        public int GroupId { get; }

        public GetPractitionersInGroupQuery(int groupId) => GroupId = groupId;
    }
}