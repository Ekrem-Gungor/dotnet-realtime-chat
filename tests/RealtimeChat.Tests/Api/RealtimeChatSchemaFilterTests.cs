using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using RealtimeChat.Api.OpenApi;
using RealtimeChat.Application.Features.Auths.Commands;
using RealtimeChat.Application.Features.Chats.Commands;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RealtimeChat.Tests.Api;

public sealed class RealtimeChatSchemaFilterTests
{
    private readonly RealtimeChatSchemaFilter _filter = new("realtime.demo");

    [Fact]
    public void Apply_ForLoginCommand_UsesSafeDemoPlaceholders()
    {
        OpenApiSchema schema = new();

        _filter.Apply(schema, CreateContext(typeof(LoginUserCommand)));

        OpenApiObject example = Assert.IsType<OpenApiObject>(schema.Example);
        Assert.Equal(
            "realtime.demo",
            Assert.IsType<OpenApiString>(example["userName"]).Value);
        Assert.DoesNotContain(
            "Local_demo_password",
            Assert.IsType<OpenApiString>(example["password"]).Value);
    }

    [Fact]
    public void Apply_ForCreateMessageCommand_HidesServerControlledSender()
    {
        OpenApiSchema schema = new()
        {
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["senderUserName"] = new(),
                ["message"] = new()
            },
            Required = new HashSet<string> { "senderUserName", "message" }
        };

        _filter.Apply(
            schema,
            CreateContext(typeof(CreateChatMessageCommand)));

        Assert.DoesNotContain("senderUserName", schema.Properties.Keys);
        Assert.DoesNotContain("senderUserName", schema.Required);
        Assert.Contains("message", schema.Properties.Keys);
    }

    private static SchemaFilterContext CreateContext(Type modelType)
    {
        return new SchemaFilterContext(
            modelType,
            schemaGenerator: null!,
            schemaRepository: new SchemaRepository());
    }
}
