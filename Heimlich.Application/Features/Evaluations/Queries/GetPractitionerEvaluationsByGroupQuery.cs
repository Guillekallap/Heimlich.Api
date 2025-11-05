using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Evaluations.Queries
{
    public class GetPractitionerEvaluationsByGroupQuery : IRequest<List<GroupEvaluationsDto>>
    {
        public string PractitionerId { get; }

        public GetPractitionerEvaluationsByGroupQuery(string practitionerId)
        {
            PractitionerId = practitionerId;
        }
    }
}
