using System.Collections.Generic;
using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Evaluations.Queries
{
    public class GetEvaluationsForInstructorQuery : IRequest<IEnumerable<EvaluationDto>>
    {
        public string InstructorId { get; }
        public GetEvaluationsForInstructorQuery(string instructorId)
        {
            InstructorId = instructorId;
        }
    }
}
