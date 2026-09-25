using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Chats.Commands;
using RealtimeChat.Application.Features.Chats.Dtos;
using RealtimeChat.Application.Features.Chats.Queries;
using RealtimeChat.Domain.CachingModels;

namespace RealtimeChat.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public sealed class ChatController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChatController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a message as the authenticated user.
    /// </summary>
    /// <remarks>
    /// The sender identity is taken from the JWT and cannot be supplied by the client.
    /// </remarks>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ChatMessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ChatMessageDto>> CreateChatMessage([FromBody] CreateChatMessageCommand command, CancellationToken cancellationToken)
    {
        command.SenderUserName = User.Identity?.Name ?? throw new UnauthorizedAccessException("Authenticated user name is missing.");

        ChatMessageDto result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Returns the recent Redis-backed message history.
    /// </summary>
    [HttpGet("messages")]
    [ProducesResponseType(typeof(List<ChatMessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ChatMessageDto>>> GetAllChatMessages(CancellationToken cancellationToken)
    {
        List<ChatMessageDto> result = await _mediator.Send(new GetAllChatMessagesQuery(), cancellationToken);
        return Ok(result);
    }
}
