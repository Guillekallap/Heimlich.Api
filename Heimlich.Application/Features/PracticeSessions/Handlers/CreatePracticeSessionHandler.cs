using Heimlich.Application.DTOs;
using Heimlich.Application.Features.PracticeSessions.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Infrastructure.Identity;
using MediatR;

namespace Heimlich.Application.Features.PracticeSessions.Handlers
{
    public class CreatePracticeSessionHandler : IRequestHandler<CreatePracticeSessionCommand, PracticeSessionDto>
    {
        private readonly HeimlichDbContext _context;

        public CreatePracticeSessionHandler(HeimlichDbContext context) => _context = context;

        public async Task<PracticeSessionDto> Handle(CreatePracticeSessionCommand request, CancellationToken cancellationToken)
        {
            var entity = new PracticeSession
            {
                PracticeType = request.PracticeType,
                CreationDate = request.Dto.CreationDate
            };
            _context.PracticeSessions.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return new PracticeSessionDto
            {
                Id = entity.Id,
                PracticeType = request.PracticeType,
                CreationDate = request.Dto.CreationDate
            };
        }
    }
}