namespace RealtimeChat.Api.Contracts.Auth
{
    public sealed record AuthenticatedUserResponse(
    int UserId,
    string UserName,
    string? Email,
    string[] Roles);
}
