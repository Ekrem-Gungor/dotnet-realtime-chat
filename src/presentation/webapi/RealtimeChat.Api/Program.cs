using Autofac;
using Autofac.Extensions.DependencyInjection;
using RealtimeChat.Api.Hubs;
using RealtimeChat.Application.Features.Auths.Commands;
using RealtimeChat.DependencyInjection.Bootstrappers;
using RealtimeChat.DependencyInjection.CustomServiceInjections;
using RealtimeChat.Infrastructure.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(s =>
{
    s.IdleTimeout = TimeSpan.FromDays(1);
    s.Cookie.HttpOnly = true;
    s.Cookie.IsEssential = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCustomIdentityServices();
builder.Services.AddJwtAuthtentication(builder.Configuration, "RealtimeChatAccessToken");
builder.Services.AddMapperInjection();
builder.Services.AddRedisConnectionProvider(builder.Configuration);

// Autofac DI Container
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
// AutoFac modüllerini yükleme
builder.Host.ConfigureContainer<ContainerBuilder>(Bootstrapper.ConfigureServices);

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(typeof(LoginUserCommandHandler).Assembly);
    config.RegisterServicesFromAssemblyContaining<Program>();
});
builder.Services.AddSignalR();

string? corsPath = builder.Configuration["UICORSPath"];

#region azureEnv
string? azureCorsPath = Environment.GetEnvironmentVariable("AzureUICORSPath");
if (!string.IsNullOrWhiteSpace(azureCorsPath)) corsPath = azureCorsPath;
#endregion

if (string.IsNullOrWhiteSpace(corsPath))
    throw new InvalidOperationException("UICORSPath configuration is required.");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins(corsPath); // React UI portu
    });
});

WebApplication app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    UserQuotaInitializer initializer = scope.ServiceProvider.GetRequiredService<UserQuotaInitializer>();
    await initializer.InitializeAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chatHub");

app.Run();
