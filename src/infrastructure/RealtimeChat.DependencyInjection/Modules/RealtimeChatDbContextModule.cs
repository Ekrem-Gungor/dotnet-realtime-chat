using Autofac;
using RealtimeChat.Persistence.ContextClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeChat.DependencyInjection.Persistence
{
    public class RealtimeChatDbContextModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(b =>
            {
                IConfiguration conf = b.Resolve<IConfiguration>();
                string connectionString = conf.GetSection("ConnectionStrings:RealtimeChatConnection").Value;
                #region Deploy edildiğinde Azure ortam değişkenlerini aktarır.
                string azureConnection = conf.GetSection("ConnectionStrings:AzureConnection").Value;
                if (azureConnection != null) connectionString = azureConnection;
                #endregion
                DbContextOptionsBuilder<RealtimeChatDbContext> optionsBuilder = new();
                optionsBuilder.UseSqlServer(connectionString).UseLazyLoadingProxies();
                return new RealtimeChatDbContext(optionsBuilder.Options);
            }).As<RealtimeChatDbContext>().AsSelf().InstancePerLifetimeScope();
        }
    }
}
