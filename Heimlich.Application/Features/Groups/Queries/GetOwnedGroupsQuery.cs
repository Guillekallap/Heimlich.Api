using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Groups.Queries
{
    public class GetOwnedGroupsQuery : IRequest<List<GroupDto>>
    {
        public string InstructorId { get; }

        public GetOwnedGroupsQuery(string instructorId)
        { InstructorId = instructorId; }
    }
}