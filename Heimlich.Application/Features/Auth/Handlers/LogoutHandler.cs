using Heimlich.Application.Features.Auth.Commands;
using Heimlich.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class LogoutHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly SignInManager<User> _signInManager;

    public LogoutHandler(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _signInManager.SignOutAsync();
        return true;
    }
}