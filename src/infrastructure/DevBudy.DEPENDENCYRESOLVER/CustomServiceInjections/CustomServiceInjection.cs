using DevBudy.APPLICATION.Mapping;
using DevBudy.APPLICATION.Services.RedisServices;
using DevBudy.COMMON.Tools.JwtSettings;
using DevBudy.DOMAIN.Entities.Concretes;
using DevBudy.INNERINFRASTRUCTURE.Redis;
using DevBudy.INNERINFRASTRUCTURE.Services.RedisServices;
using DevBudy.PERSISTANCE.ContextClasses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.DEPENDENCYRESOLVER.CustomServiceInjections
{
    public static class CustomServiceInjection
    {
        public static void AddCustomIdentityServices(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequiredLength = 8;
                opt.SignIn.RequireConfirmedEmail = true;
                opt.Password.RequireNonAlphanumeric = true;
            }).AddEntityFrameworkStores<DevBudyContext>().AddDefaultTokenProviders();
        }

        public static IServiceCollection AddJwtAuthtentication(this IServiceCollection services, IConfiguration configuration, string cookieKeyName)
        {
            IConfiguration jwtSettingsSection = configuration.GetSection("JwtSettings");
            services.Configure<JwtSetting>(jwtSettingsSection);
            JwtSetting jwtSettings = jwtSettingsSection.Get<JwtSetting>()
                ?? throw new InvalidOperationException("JwtSettings configuration is required.");

            if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey) || jwtSettings.SecretKey.Length < 32)
                throw new InvalidOperationException("JwtSettings:SecretKey must contain at least 32 characters.");

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Token süresinin bitiminden sonra 0 saniye bekler
                };

                opt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[cookieKeyName];

                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
            return services;
        }

        public static void AddMapperInjection(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new MapProfile());
            }, Assembly.GetExecutingAssembly());
        }

        public static void AddRedisConnectionProvider(this IServiceCollection services, IConfiguration configuration)
        {
            string redisConnection = configuration["Redis"]
                ?? throw new InvalidOperationException("Redis configuration is required.");

            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnection));
            services.AddSingleton<IRedisConnectionProvider, RedisConnectionProvider>();
            services.AddScoped<UserQuotaInitializer>();
        }
    }
}
