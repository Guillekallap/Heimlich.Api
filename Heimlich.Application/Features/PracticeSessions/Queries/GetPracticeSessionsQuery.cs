using Heimlich.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Heimlich.Application.Features.PracticeSessions.Queries
{
    public class GetPracticeSessionsQuery : IRequest<IEnumerable<PracticeSessionDto>> { }
}