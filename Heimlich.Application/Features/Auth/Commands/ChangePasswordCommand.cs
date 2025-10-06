using MediatR;

namespace Heimlich.Application.Features.Auth.Commands
{
    public class ChangePasswordCommand : IRequest<bool>
    {
        public string UserId { get; }
        public string CurrentPassword { get; }
        public string NewPassword { get; }

        public ChangePasswordCommand(string userId, string currentPassword, string newPassword)
        {
            UserId = userId;
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
        }
    }
}