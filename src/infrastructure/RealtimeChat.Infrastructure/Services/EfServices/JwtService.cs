using RealtimeChat.Application.Features.Auths.Dtos.Response;
using RealtimeChat.Application.Services.EFServices;
using RealtimeChat.Common.Tools.JwtSettings;
using RealtimeChat.Domain.Entities.Concretes;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.Infrastructure.Services.EfServices
{
    public class JwtService : IJwtService
    {
        private readonly JwtSetting _jwtSetting;
        private readonly UserManager<AppUser> _userManager;

        public JwtService(UserManager<AppUser> userManager, IOptions<JwtSetting> jwtSetting)
        {
            _userManager = userManager;
            _jwtSetting = jwtSetting.Value;
        }

        public async Task<TokenResponseDto> GenerateToken(string userId)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new ArgumentException("User not found");
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            IList<string> userRoles = await _userManager.GetRolesAsync(user);
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtSetting.SecretKey));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);
            double expireMinutes = _jwtSetting.ExpireMinutes;

            JwtSecurityToken token = new(
                issuer: _jwtSetting.Issuer,
                audience: _jwtSetting.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResponseDto()
            {
                Token = tokenString,
                ExpiresDate = token.ValidTo
            };
        }
    }
}
