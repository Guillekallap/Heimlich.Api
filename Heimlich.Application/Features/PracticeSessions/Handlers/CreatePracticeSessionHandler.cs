using Heimlich.Application.DTOs;
using Heimlich.Application.Features.PracticeSessions.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Infrastructure;
using MediatR;
using System;

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
                Title = request.Title,
                ScheduledAt = request.ScheduledAt
            };
            _context.PracticeSessions.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return new PracticeSessionDto
            {
                Id = entity.Id,
                Title = entity.Title,
                ScheduledAt = entity.ScheduledAt
            };
        }
    }
}
