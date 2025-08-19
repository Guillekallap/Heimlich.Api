using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Infrastructure;
using MediatR;

namespace Heimlich.Application.Features.Groups.Handlers
{
    public class CreateGroupHandler : IRequestHandler<CreateGroupCommand, GroupDto>
    {
        private readonly HeimlichDbContext _context;

        public CreateGroupHandler(HeimlichDbContext context) => _context = context;

        public async Task<GroupDto> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var entity = new Group { Name = request.Name };
            _context.Groups.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return new GroupDto { Id = entity.Id, Name = entity.Name };
        }
    }
}