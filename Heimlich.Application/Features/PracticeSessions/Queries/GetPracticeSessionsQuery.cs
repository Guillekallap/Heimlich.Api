using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.PracticeSessions.Queries
{
    public class GetPracticeSessionsQuery : IRequest<IEnumerable<PracticeSessionDto>>
    { }
}