namespace RealtimeChat.DependencyInjection.DemoIdentity;

public sealed class DemoIdentityOptions
{
    public const string SectionName = "DemoIdentity";

    public bool Enabled { get; set; }
    public string UserName { get; set; } = "realtime.demo";
    public string Email { get; set; } = "realtime.demo@example.invalid";
    public string Password { get; set; } = string.Empty;
}
