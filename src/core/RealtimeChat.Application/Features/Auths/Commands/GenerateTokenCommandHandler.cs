using MediatR;
using RealtimeChat.Application.Features.Auths.Dtos.Response;
using RealtimeChat.Application.Services.EFServices;

namespace RealtimeChat.Application.Features.Auths.Commands;

public sealed class GenerateTokenCommandHandler : IRequestHandler<GenerateTokenCommand, TokenResponseDto>
{
    private readonly IJwtService _jwtService;

    public GenerateTokenCommandHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public Task<TokenResponseDto> Handle(
        GenerateTokenCommand request,
        CancellationToken cancellationToken)
    {
        return _jwtService.GenerateToken(request.UserId.ToString());
    }
}
