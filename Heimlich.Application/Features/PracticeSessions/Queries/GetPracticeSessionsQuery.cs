using Heimlich.Application.DTOs;
using Heimlich.Domain.Enums;
using MediatR;

namespace Heimlich.Application.Features.PracticeSessions.Queries
{
    public class GetPracticeSessionsQuery : IRequest<List<PracticeSessionDto>>
    {
        public string PractitionerId { get; set; }
        public PracticeTypeEnum? PracticeType { get; set; }
        // Si PracticeType es null, devuelve todas las sesiones

        public GetPracticeSessionsQuery(string userId, PracticeTypeEnum simulation)
        {
            PractitionerId = userId;
            PracticeType = simulation;
        }
    }
}