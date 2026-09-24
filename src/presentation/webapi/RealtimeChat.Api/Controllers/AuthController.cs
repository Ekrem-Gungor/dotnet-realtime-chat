using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Auths.Commands;
using RealtimeChat.Application.Features.Auths.Dtos.Response;
using RealtimeChat.DependencyInjection.Authentication;

namespace RealtimeChat.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponseDto>> Login(
        [FromBody] LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        LoginResponseDto loginResult = await _mediator.Send(command, cancellationToken);
        TokenResponseDto tokenResult = await _mediator.Send(new GenerateTokenCommand { UserId = loginResult.UserId }, cancellationToken);

        Response.Cookies.Append(
            JwtAuthenticationDefaults.AccessTokenCookieName,
            tokenResult.Token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = tokenResult.ExpiresDate,
                Path = "/"
            });

        return Ok(tokenResult);
    }
}
