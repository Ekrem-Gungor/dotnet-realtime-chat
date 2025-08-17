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
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequiredLength = 5;
                opt.SignIn.RequireConfirmedEmail = true;
                opt.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<DevBudyContext>().AddDefaultTokenProviders();
        }

        public static IServiceCollection AddJwtAuthtentication(this IServiceCollection services, IConfiguration configuration, string cookieKeyName)
        {
            IConfiguration jwtSettingsSection = configuration.GetSection("JwtSettings");
            services.Configure<JwtSetting>(jwtSettingsSection);
            JwtSetting jwtSettings = jwtSettingsSection.Get<JwtSetting>();

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.RequireHttpsMetadata = false;
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
            services.AddSingleton<IConnectionMultiplexer>(cm => ConnectionMultiplexer.Connect(configuration["Redis"]));
            services.AddSingleton<IRedisConnectionProvider, RedisConnectionProvider>();
            services.AddScoped<UserQuotaInitializer>();
        }
    }
}
