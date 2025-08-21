using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Auth.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Heimlich.Application.Features.Auth.Handlers
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, RegisterResponseDto>
    {
        private readonly UserManager<User> _userManager;

        public RegisterHandler(UserManager<User> userManager)
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

            var user = new User { UserName = request.UserName, Email = request.Email, Fullname = request.FullName, CreationDate = DateTime.Now };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return new RegisterResponseDto { Error = "Error al crear el usuario" };
            }
            var roleName = request.Role.ToString();
            await _userManager.AddToRoleAsync(user, roleName);
            return new RegisterResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                UserName = request.UserName,
                FullName = user.Fullname,
                Role = roleName
            };
        }
    }
}