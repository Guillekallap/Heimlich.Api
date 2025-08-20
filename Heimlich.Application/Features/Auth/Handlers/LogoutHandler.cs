using Heimlich.Application.Features.Auth.Commands;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class LogoutHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LogoutHandler(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _signInManager.SignOutAsync();
        return true;
    }
}