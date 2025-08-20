using Heimlich.Application.DTOs;
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
            if (request.Password.Length < 8)
                return new RegisterResponseDto { Error = "La contraseña debe tener al menos 8 caracteres" };

            if (await _userManager.FindByNameAsync(request.UserName) != null)
                return new RegisterResponseDto { Error = "El usuario ya existe" };

            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return new RegisterResponseDto { Error = "El correo ya existe" };

            var user = new ApplicationUser { UserName = request.UserName, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return new RegisterResponseDto { Error = "Error al crear el usuario" };
            }
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