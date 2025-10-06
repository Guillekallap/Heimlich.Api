using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Users.Queries
{
    public class GetUserProfileQuery : IRequest<UserDto>
    {
        public string UserId { get; }

        public GetUserProfileQuery(string userId)
        { UserId = userId; }
    }
}