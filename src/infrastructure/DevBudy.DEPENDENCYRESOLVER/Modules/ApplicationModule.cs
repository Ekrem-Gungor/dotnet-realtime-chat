using Autofac;
using DevBudy.INNERINFRASTRUCTURE.Services.EfServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Module = Autofac.Module;

namespace DevBudy.DEPENDENCYRESOLVER.Modules
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = new[]
            {
                Assembly.GetExecutingAssembly(),
                Assembly.GetAssembly(typeof(BaseService<,>))
            };

            //#region ConsoleLogRegisteredServices
            //IEnumerable<Type> types = assemblies.SelectMany(a => a.GetTypes()).Where(t => t.Name.EndsWith("Service") && t.IsClass);
            //foreach (Type type in types)
            //{
            //    Console.WriteLine($"Service Registered: {type.Name}");
            //}
            //#endregion

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.Name.EndsWith("Service") && t.IsClass)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
