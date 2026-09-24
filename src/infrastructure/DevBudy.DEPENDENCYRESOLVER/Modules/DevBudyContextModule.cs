using Autofac;
using DevBudy.PERSISTANCE.ContextClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.DEPENDENCYRESOLVER.Persistances
{
    public class DevBudyContextModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(b =>
            {
                IConfiguration conf = b.Resolve<IConfiguration>();
                string connectionString = conf.GetSection("ConnectionStrings:DevBudyConnection").Value;
                #region Deploy edildiğinde Azure ortam değişkenlerini aktarır.
                string azureConnection = conf.GetSection("ConnectionStrings:AzureConnection").Value;
                if (azureConnection != null) connectionString = azureConnection;
                #endregion
                DbContextOptionsBuilder<DevBudyContext> optionsBuilder = new();
                optionsBuilder.UseSqlServer(connectionString).UseLazyLoadingProxies();
                return new DevBudyContext(optionsBuilder.Options);
            }).As<DevBudyContext>().AsSelf().InstancePerLifetimeScope();
        }
    }
}
