using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RealtimeChat.Application.Common.Behaviors;
using RealtimeChat.Application.Features.Auths.Commands;
using RealtimeChat.Application.Services.EFServices;
using RealtimeChat.Common.Tools.JwtSettings;
using RealtimeChat.Contracts.Repositories.EFRepositories;
using RealtimeChat.Contracts.Repositories.RedisRepositories;
using RealtimeChat.DependencyInjection.Authentication;
using RealtimeChat.DependencyInjection.DemoIdentity;
using RealtimeChat.Domain.Entities.Concretes;
using RealtimeChat.Infrastructure.Redis.Repositories;
using RealtimeChat.Infrastructure.Services.EfServices;
using RealtimeChat.Persistence.ContextClasses;
using RealtimeChat.Persistence.Repositories.EFRepositories;
using StackExchange.Redis;
using System.Reflection;
using System.Text;

namespace RealtimeChat.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRealtimeChat(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] additionalHandlerAssemblies)
    {
        services.AddDatabase(configuration);
        services.AddIdentityServices();
        services.AddJwtAuthentication(configuration);
        services.AddApplicationServices(additionalHandlerAssemblies);
        services.AddRepositories();
        services.AddRedis(configuration);
        services.AddDemoIdentity(configuration);

        return services;
    }

    private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("RealtimeChatConnection")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:RealtimeChatConnection configuration is required.");

        services.AddDbContext<RealtimeChatDbContext>(options =>
            options
                .UseSqlServer(connectionString)
                .UseLazyLoadingProxies());
    }

    private static void AddIdentityServices(this IServiceCollection services)
    {
        services
            .AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<RealtimeChatDbContext>()
            .AddDefaultTokenProviders();
    }

    private static void AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        IConfigurationSection jwtSection = configuration.GetRequiredSection("JwtSettings");
        JwtSetting jwtSettings = jwtSection.Get<JwtSetting>()
            ?? throw new InvalidOperationException("JwtSettings configuration is required.");

        if (string.IsNullOrWhiteSpace(jwtSettings.Issuer)
            || string.IsNullOrWhiteSpace(jwtSettings.Audience)
            || string.IsNullOrWhiteSpace(jwtSettings.SecretKey)
            || jwtSettings.SecretKey.Length < 32
            || jwtSettings.ExpireMinutes <= 0)
        {
            throw new InvalidOperationException("JwtSettings contains invalid values.");
        }

        services.Configure<JwtSetting>(jwtSection);

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        string? hubToken = context.Request.Query["access_token"];

                        if (!string.IsNullOrWhiteSpace(hubToken)
                            && context.HttpContext.Request.Path.StartsWithSegments(
                                JwtAuthenticationDefaults.HubPath))
                        {
                            context.Token = hubToken;
                        }
                        else if (context.Request.Cookies.TryGetValue(
                                     JwtAuthenticationDefaults.AccessTokenCookieName,
                                     out string? cookieToken))
                        {
                            context.Token = cookieToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();
    }

    private static void AddApplicationServices(this IServiceCollection services, Assembly[] additionalHandlerAssemblies)
    {
        Assembly applicationAssembly = typeof(LoginUserCommandHandler).Assembly;

        Assembly[] handlerAssemblies =
        [
            applicationAssembly,
            .. additionalHandlerAssemblies
        ];

        services.AddValidatorsFromAssembly(applicationAssembly);

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(handlerAssemblies);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddScoped<IJwtService, JwtService>();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IAppRoleRepository, AppRoleRepository>();
        services.AddScoped<IAppUserProfileRepository, AppUserProfileRepository>();
        services.AddScoped<IAppUserRepository, AppUserRepository>();
        services.AddScoped<IAppUserRoleRepository, AppUserRoleRepository>();
        services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
        services.AddScoped<IMessageRedisRepository, RedisMessageRepository>();
        services.AddScoped<IMessageQuotaRepository, MessageQuotaRepository>();
    }

    private static void AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        string redisConnection = configuration["Redis"]
            ?? throw new InvalidOperationException("Redis configuration is required.");

        // ConnectionMultiplexer eş zamanlı kullanıma uygun; her istek için bağlantı açılmaması gerekir.
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(redisConnection));
    }

    private static void AddDemoIdentity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<DemoIdentityOptions>(
            configuration.GetSection(DemoIdentityOptions.SectionName));
        services.AddScoped<DevelopmentDemoIdentityInitializer>();
    }
}
