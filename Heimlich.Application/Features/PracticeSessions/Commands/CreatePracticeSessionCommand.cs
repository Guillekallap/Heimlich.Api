using Heimlich.Application.DTOs;
using Heimlich.Domain.Enums;
using MediatR;

namespace Heimlich.Application.Features.PracticeSessions.Commands
{
    public class CreatePracticeSessionCommand : IRequest<PracticeSessionDto>
    {
        public CreatePracticeSessionDto Dto { get; }
        public PracticeTypeEnum PracticeType { get; }
        public string PractitionerId { get; }

        public CreatePracticeSessionCommand(CreatePracticeSessionDto dto, PracticeTypeEnum practiceType, string practitionerId)
        {
            Dto = dto;
            PracticeType = practiceType;
            PractitionerId = practitionerId;
        }
    }
}