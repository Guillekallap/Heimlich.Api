using System.Collections.Generic;
using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Evaluations.Queries
{
    public class GetRankingForInstructorQuery : IRequest<IEnumerable<RankingDto>>
    {
        public string InstructorId { get; }
        public GetRankingForInstructorQuery(string instructorId)
        {
            InstructorId = instructorId;
        }
    }
}
