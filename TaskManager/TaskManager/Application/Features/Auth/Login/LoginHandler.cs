using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManager.Application.Domain.Entities;
using TaskManager.Config;
using TaskManager.Exceptions;

namespace TaskManager.Application.Features.Auth.Login;

public class LoginHandler(IOptions<JwtSettings> jwtSettings, UserManager<User> userManager) : IRequestHandler<LoginCommand, string>
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly UserManager<User> _userManager = userManager;

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username) 
            ?? throw new ValidationAppException("UNAUTHORIZED", "Неверный логин или пароль");

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordValid)
        {
            throw new ValidationAppException("UNAUTHORIZED", "Неверный логин или пароль");
        }

        var userId = await _userManager.GetUserIdAsync(user);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.Role, "User"),
                new Claim(ClaimTypes.NameIdentifier, userId)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return jwtToken;
    }
}
