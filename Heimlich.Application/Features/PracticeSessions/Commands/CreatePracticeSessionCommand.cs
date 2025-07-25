using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.PracticeSessions.Commands
{
    public class CreatePracticeSessionCommand : IRequest<PracticeSessionDto>
    {
        public string Title { get; set; }
        public DateTime ScheduledAt { get; set; }
    }
}