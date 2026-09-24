using RealtimeChat.Application.Features.Auths.Dtos.Response;
using RealtimeChat.Application.Features.Auths.Queries;
using RealtimeChat.Application.Features.Chats.Commands;
using RealtimeChat.Application.Features.Chats.Queries;
using RealtimeChat.Application.Services.RedisServices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RealtimeChat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        readonly IMediator _mediator;
        readonly IMessageQuotaService _msgQuotaService;

        public ChatController(IMediator mediator, IMessageQuotaService msgQuotaService)
        {
            _mediator = mediator;
            _msgQuotaService = msgQuotaService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateChatMessage(CreateChatMessageCommand command)
        {
            command.SenderUserName = User.Identity?.Name
                ?? throw new UnauthorizedAccessException("Authenticated user name is missing.");

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("messages")]
        public async Task<IActionResult> GetAllChatMessages()
        {
            var result = await _mediator.Send(new GetAllChatMessagesQuery());
            return Ok(result);
        }
    }
}
