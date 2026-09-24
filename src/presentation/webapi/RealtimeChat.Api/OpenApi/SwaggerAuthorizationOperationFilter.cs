using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace RealtimeChat.Api.OpenApi;

public sealed class SwaggerAuthorizationOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!RequiresAuthorization(context.MethodInfo))
            return;

        operation.Security ??= [];
        operation.Security.Add(
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = SwaggerServiceCollectionExtensions.BearerScheme
                    }
                }] = []
            });
    }

    public static bool RequiresAuthorization(MethodInfo methodInfo)
    {
        IEnumerable<object> attributes = methodInfo
            .GetCustomAttributes(inherit: true)
            .Concat(
                methodInfo.DeclaringType?.GetCustomAttributes(inherit: true)
                ?? []);

        bool allowsAnonymousAccess = attributes.OfType<IAllowAnonymous>().Any();
        bool requiresAuthorization = attributes.OfType<IAuthorizeData>().Any();

        return requiresAuthorization && !allowsAnonymousAccess;
    }
}
