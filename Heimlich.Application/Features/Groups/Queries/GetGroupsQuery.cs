using Heimlich.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Heimlich.Application.Features.Groups.Queries
{
    public class GetGroupsQuery : IRequest<IEnumerable<GroupDto>> { }
}
