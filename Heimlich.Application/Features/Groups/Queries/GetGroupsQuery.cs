using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Groups.Queries
{
    public class GetGroupsQuery : IRequest<IEnumerable<GroupDto>>
    { }
}