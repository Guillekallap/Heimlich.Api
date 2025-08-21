using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Auth.Queries;
using Heimlich.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Heimlich.Application.Features.Auth.Handlers
{
    public class LoginHandler : IRequestHandler<LoginQuery, AuthResultDto>
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public LoginHandler(SignInManager<User> signInManager, UserManager<User> userManager, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthResultDto> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            User user = null;
            if (!string.IsNullOrEmpty(request.Email))
                user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null && !string.IsNullOrEmpty(request.UserName))
                user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
                return new AuthResultDto { Error = "Usuario inexistente" };

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return new AuthResultDto { Error = "Contraseña incorrecta" };

            // Crear JWT con roles del usuario
            var roles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
            };
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: creds);

            return new AuthResultDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiry = token.ValidTo
            };
        }
    }
}