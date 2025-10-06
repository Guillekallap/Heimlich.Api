using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Users.Queries;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Heimlich.Application.Features.Users.Handlers
{
    public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, UserDto>
    {
        private readonly HeimlichDbContext _context;
        private readonly UserManager<Heimlich.Domain.Entities.User> _userManager;

        public GetUserProfileHandler(HeimlichDbContext context, UserManager<Heimlich.Domain.Entities.User> userManager)
        { _context = context; _userManager = userManager; }

        public async Task<UserDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
            if (user == null) return null;
            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Mail = user.Email,
                Fullname = user.Fullname,
                Role = roles.FirstOrDefault(),
                CreationDate = user.CreationDate
            };
        }
    }
}