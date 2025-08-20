using Heimlich.Application.DTOs;
using Heimlich.Domain.Enums;

namespace Heimlich.Application.Features.PracticeSessions.Commands
{
    public class CancelPracticeSessionCommand
    {
        public CancelPracticeSessionCommand(int sessionId)
        {
        }

        public CancelPracticeSessionDto Dto { get; }
        public PracticeTypeEnum PracticeType { get; }
    }
}