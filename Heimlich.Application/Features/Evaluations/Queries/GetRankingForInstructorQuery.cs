using Heimlich.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Heimlich.Application.Features.Evaluations.Queries
{
    public class GetRankingForInstructorQuery : IRequest<IEnumerable<GroupRankingDto>>
    {
        public string InstructorId { get; }

        public GetRankingForInstructorQuery(string instructorId)
        {
            InstructorId = instructorId;
        }
    }
}