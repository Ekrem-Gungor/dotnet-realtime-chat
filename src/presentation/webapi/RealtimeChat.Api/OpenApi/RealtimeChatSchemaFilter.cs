using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using RealtimeChat.Application.Features.Auths.Commands;
using RealtimeChat.Application.Features.Auths.Dtos.Response;
using RealtimeChat.Application.Features.Chats.Commands;
using RealtimeChat.Application.Features.Chats.Dtos;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RealtimeChat.Api.OpenApi;

public sealed class RealtimeChatSchemaFilter : ISchemaFilter
{
    private readonly string _demoUserName;

    public RealtimeChatSchemaFilter(string demoUserName)
    {
        _demoUserName = demoUserName;
    }

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(LoginUserCommand))
        {
            schema.Example = new OpenApiObject
            {
                ["userName"] = new OpenApiString(_demoUserName),
                ["password"] = new OpenApiString("<DEMO_USER_PASSWORD from .env>")
            };

            return;
        }

        if (context.Type == typeof(TokenResponseDto))
        {
            schema.Example = new OpenApiObject
            {
                ["token"] = new OpenApiString("header.payload.signature"),
                ["expiresDate"] = new OpenApiString("2026-09-24T21:06:10Z")
            };

            return;
        }

        if (context.Type == typeof(CreateChatMessageCommand))
        {
            // Gönderen kullanıcı JWT claim'inden geldiği için istemci sözleşmesinde yer almaz.
            schema.Properties.Remove("senderUserName");
            schema.Required?.Remove("senderUserName");
            schema.Example = new OpenApiObject
            {
                ["message"] = new OpenApiString("Hello from Siglora!")
            };

            return;
        }

        if (context.Type == typeof(ChatMessageDto))
        {
            schema.Example = new OpenApiObject
            {
                ["id"] = new OpenApiString("f9f53075-fbba-4f23-a817-0a4e76ea17e1"),
                ["senderUserId"] = new OpenApiInteger(4),
                ["senderUserName"] = new OpenApiString(_demoUserName),
                ["message"] = new OpenApiString("Hello from Siglora!"),
                ["sendAt"] = new OpenApiString("2026-09-24T20:06:31Z")
            };
        }
    }
}
