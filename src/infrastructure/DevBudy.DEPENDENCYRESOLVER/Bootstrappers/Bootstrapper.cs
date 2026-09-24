using Autofac;
using DevBudy.DEPENDENCYRESOLVER.Modules;
using DevBudy.DEPENDENCYRESOLVER.Persistances;
using DevBudy.DOMAIN.Entities.Concretes;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.DEPENDENCYRESOLVER.Bootstrappers
{
    public static class Bootstrapper
    {
        public static void ConfigureServices(ContainerBuilder builder)
        {
            //builder.RegisterModule(new IdentityServiceInjectionModule()); todo : Burada bir takım sorunlar var incelenecek !
            builder.RegisterModule(new PersistanceModule());
            builder.RegisterModule(new ApplicationModule());
            builder.RegisterModule(new DevBudyContextModule());
        }
    }
}
