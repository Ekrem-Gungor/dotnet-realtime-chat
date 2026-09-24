using Autofac;
using RealtimeChat.DependencyInjection.Modules;
using RealtimeChat.DependencyInjection.Persistence;
using RealtimeChat.Domain.Entities.Concretes;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.DependencyInjection.Bootstrappers
{
    public static class Bootstrapper
    {
        public static void ConfigureServices(ContainerBuilder builder)
        {
            //builder.RegisterModule(new IdentityServiceInjectionModule()); todo : Burada bir takım sorunlar var incelenecek !
            builder.RegisterModule(new PersistenceModule());
            builder.RegisterModule(new ApplicationModule());
            builder.RegisterModule(new RealtimeChatDbContextModule());
        }
    }
}
