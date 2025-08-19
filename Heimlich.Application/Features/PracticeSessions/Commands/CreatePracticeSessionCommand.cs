using Heimlich.Application.DTOs;
using Heimlich.Domain.Entities;
using MediatR;

namespace Heimlich.Application.Features.PracticeSessions.Commands
{
    public class CreatePracticeSessionCommand : IRequest<PracticeSessionDto>
    {
        public PracticeType PracticeType { get; set; }
        public DateTime CreationDate { get; set; }
    }
}