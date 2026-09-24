using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Chats.Commands;
using RealtimeChat.Application.Features.Chats.Dtos;
using RealtimeChat.Application.Features.Chats.Queries;

namespace RealtimeChat.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public sealed class ChatController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChatController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<ActionResult<ChatMessageDto>> CreateChatMessage(
        CreateChatMessageCommand command,
        CancellationToken cancellationToken)
    {
        command.SenderUserName = User.Identity?.Name ?? throw new UnauthorizedAccessException("Authenticated user name is missing.");

        ChatMessageDto result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("messages")]
    public async Task<IActionResult> GetAllChatMessages(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllChatMessagesQuery(), cancellationToken);
        return Ok(result);
    }
}
