using RealtimeChat.Application.Features.Auths.Commands;
using RealtimeChat.Application.Features.Auths.Dtos.Response;
using RealtimeChat.Application.Services.EFServices;

namespace RealtimeChat.Tests.Application;

public sealed class GenerateTokenCommandHandlerTests
{
    [Fact]
    public async Task Handle_DelegatesTokenCreationWithoutHttpDependency()
    {
        TokenResponseDto expected = new()
        {
            Token = "test-token",
            ExpiresDate = DateTime.UtcNow.AddMinutes(30)
        };
        StubJwtService jwtService = new(expected);
        GenerateTokenCommandHandler handler = new(jwtService);

        TokenResponseDto result = await handler.Handle(
            new GenerateTokenCommand { UserId = 42 },
            CancellationToken.None);

        Assert.Same(expected, result);
        Assert.Equal("42", jwtService.ReceivedUserId);
    }

    private sealed class StubJwtService : IJwtService
    {
        private readonly TokenResponseDto _response;

        public StubJwtService(TokenResponseDto response)
        {
            _response = response;
        }

        public string? ReceivedUserId { get; private set; }

        public Task<TokenResponseDto> GenerateToken(string userId)
        {
            ReceivedUserId = userId;
            return Task.FromResult(_response);
        }
    }
}
