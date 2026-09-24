using DevBudy.APPLICATION.Features.Auths.Dtos.Response;
using DevBudy.APPLICATION.Services.EFServices;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.APPLICATION.Features.Auths.Commands
{
    public class GenerateTokenCommandHandler : IRequestHandler<GenerateTokenCommand, TokenResponseDto>
    {
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GenerateTokenCommandHandler(IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
        {
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TokenResponseDto> Handle(GenerateTokenCommand request, CancellationToken cancellationToken)
        {
            TokenResponseDto result = await _jwtService.GenerateToken(request.UserId.ToString());

            HttpContext httpContext = _httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("HTTP context is not available.");

            httpContext.Response.Cookies.Append("DevBudyAccessToken", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = httpContext.Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = result.ExpiresDate,
                Path = "/"
            });

            return result;
        }
    }
}
