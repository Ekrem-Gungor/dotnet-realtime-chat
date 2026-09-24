using Microsoft.OpenApi.Models;

namespace RealtimeChat.Api.OpenApi;

public static class SwaggerServiceCollectionExtensions
{
    public const string DocumentName = "v1";
    public const string BearerScheme = "Bearer";

    public static IServiceCollection AddRealtimeChatSwagger(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string demoUserName = configuration["DemoIdentity:UserName"]
            ?? "realtime.demo";

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                DocumentName,
                new OpenApiInfo
                {
                    Title = "Talkio API",
                    Version = DocumentName,
                    Description =
                        $"Authenticate with POST /api/Auth/login using the local demo user " +
                        $"'{demoUserName}' and the DEMO_USER_PASSWORD value from .env. " +
                        "Copy the token from the response, select Authorize, and paste only " +
                        "the token. Swagger adds the Bearer prefix automatically."
                });

            options.AddSecurityDefinition(
                BearerScheme,
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Paste the JWT returned by POST /api/Auth/login. Do not include the Bearer prefix.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

            // Yalnızca [Authorize] kullanan endpoint'lerde kilit simgesi gösterilir.
            options.OperationFilter<SwaggerAuthorizationOperationFilter>();
            options.SchemaFilter<RealtimeChatSchemaFilter>(demoUserName);

            string xmlDocumentationPath = Path.Combine(
                AppContext.BaseDirectory,
                "RealtimeChat.Api.xml");

            options.IncludeXmlComments(xmlDocumentationPath);
        });

        return services;
    }
}
