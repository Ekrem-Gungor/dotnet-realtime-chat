using RealtimeChat.Api.ErrorHandling;
using RealtimeChat.Api.Hubs;
using RealtimeChat.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddHealthChecks();
builder.Services.AddRealtimeChat(builder.Configuration, typeof(Program).Assembly);

string corsOrigin = builder.Configuration["UICORSPath"]
    ?? throw new InvalidOperationException("UICORSPath configuration is required.");

if (!Uri.TryCreate(corsOrigin, UriKind.Absolute, out _))
    throw new InvalidOperationException("UICORSPath configuration is required.");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins(corsOrigin);
    });
});

WebApplication app = builder.Build();

await app.ApplyDatabaseMigrationsAsync();
await app.BootstrapDemoIdentityAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseCors("AllowReactApp");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.MapHub<ChatHub>("/chatHub");

app.Run();
