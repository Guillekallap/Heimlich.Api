using Heimlich.Application.Features.Auth.Commands;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Heimlich.Application.Features.Auth.Handlers
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, RegisterResponseDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<RegisterResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser { UserName = request.UserName, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                // Manejar errores apropiadamente...
                return null;
            }
            // Asignar rol por defecto (practitioner)
            await _userManager.AddToRoleAsync(user, "Practitioner");
            return new RegisterResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                UserName = request.UserName,
                Role = "Practitioner"
            };
        }
    }
}