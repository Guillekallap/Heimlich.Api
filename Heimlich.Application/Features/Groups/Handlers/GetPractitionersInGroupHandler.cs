using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Queries;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Groups.Handlers
{
    public class GetPractitionersInGroupHandler : IRequestHandler<GetPractitionersInGroupQuery, List<UserDto>>
    {
        private readonly HeimlichDbContext _context;

        public GetPractitionersInGroupHandler(HeimlichDbContext context) => _context = context;

        public async Task<List<UserDto>> Handle(GetPractitionersInGroupQuery request, CancellationToken cancellationToken)
        {
            var practitioners = await _context.UserGroups
                .Where(ug => ug.GroupId == request.GroupId)
                .Select(ug => new UserDto
                {
                    Id = ug.UserId,
                    Fullname = ug.User.Fullname
                })
                .ToListAsync(cancellationToken);

            return practitioners;
        }
    }
}