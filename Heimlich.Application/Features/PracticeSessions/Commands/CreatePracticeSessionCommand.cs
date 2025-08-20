using Heimlich.Application.DTOs;
using Heimlich.Domain.Enums;
using MediatR;

namespace Heimlich.Application.Features.PracticeSessions.Commands
{
    public class CreatePracticeSessionCommand : IRequest<PracticeSessionDto>
    {
        public CreatePracticeSessionDto Dto { get; }
        public PracticeTypeEnum PracticeType { get; }

        public CreatePracticeSessionCommand(CreatePracticeSessionDto dto, PracticeTypeEnum practiceType)
        {
            Dto = dto;
            PracticeType = practiceType;
        }
    }
}