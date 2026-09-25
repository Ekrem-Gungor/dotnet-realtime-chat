using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Api.Contracts.Auth;
using RealtimeChat.Application.Features.Auths.Commands;
using RealtimeChat.Application.Features.Auths.Dtos.Response;
using RealtimeChat.DependencyInjection.Authentication;
using System.Security.Claims;

namespace RealtimeChat.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Authenticates a user and issues a JWT access token.
    /// </summary>
    /// <remarks>
    /// For the local Docker demo, use the configured DEMO_USER_NAME and
    /// DEMO_USER_PASSWORD values. Copy the returned token into Swagger's
    /// Authorize dialog to call protected endpoints.
    /// </remarks>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Returns the currently authenticated user.
    /// </summary>
    [HttpGet("session")]
    [Authorize]
    [ProducesResponseType(typeof(AuthenticatedUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public ActionResult<AuthenticatedUserResponse> GetSession()
    {
        string? userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out int userId))
            throw new UnauthorizedAccessException("Authenticated user identifier is missing.");

        string userName = User.Identity?.Name
            ?? throw new UnauthorizedAccessException("Authenticated user name is missing.");

        string? email = User.FindFirstValue(ClaimTypes.Email);

        string[] roles = User
            .FindAll(ClaimTypes.Role)
            .Select(claim => claim.Value)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return Ok(new AuthenticatedUserResponse(
            userId,
            userName,
            email,
            roles));
    }

    /// <summary>
    /// Clears the authentication cookie.
    /// </summary>
    [HttpPost("logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(JwtAuthenticationDefaults.AccessTokenCookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });

        return NoContent();
    }
}
