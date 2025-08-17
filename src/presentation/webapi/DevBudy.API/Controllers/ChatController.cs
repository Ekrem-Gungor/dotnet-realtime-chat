using DevBudy.APPLICATION.Features.Auths.Dtos.Response;
using DevBudy.APPLICATION.Features.Auths.Queries;
using DevBudy.APPLICATION.Features.Chats.Commands;
using DevBudy.APPLICATION.Features.Chats.Queries;
using DevBudy.APPLICATION.Services.RedisServices;
using MediatR;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevBudy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
