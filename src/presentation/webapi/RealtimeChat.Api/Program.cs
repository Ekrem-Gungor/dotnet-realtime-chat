using RealtimeChat.Api.Hubs;
using RealtimeChat.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.MapHub<ChatHub>("/chatHub");

app.Run();
