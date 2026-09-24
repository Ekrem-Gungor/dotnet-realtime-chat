using Autofac;
using DevBudy.PERSISTANCE.Repositories.EFRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Module = Autofac.Module;

namespace DevBudy.DEPENDENCYRESOLVER.Persistances
{
    public class PersistanceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = new Assembly[]
            {
                Assembly.GetExecutingAssembly(),
                Assembly.GetAssembly(typeof(Repository<>))
            };


            // Buradaki RegisterAssemblyTypes metodu, belirtilen assemblylerdeki tüm sınıfları tarar
            // ve isimleri "Repository" ile bitenleri bulur. Bu sınıflar, IRepository arayüzünü uygulayan
            // sınıflar olarak kabul edilir ve bu arayüzlerle eşleştirilir.
            // AsImplementedInterfaces, sınıfların uyguladığı tüm arayüzleri kaydeder.
            // InstancePerLifetimeScope, her istek için yeni bir örnek oluşturulmasını sağlar.

            builder.RegisterAssemblyTypes(assemblies)
                .Where(a => a.Name.EndsWith("Repository"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            // Test için kullanılmıştır.
            //#region ConsoleLogRegisteredRepositories
            //IEnumerable<Type> types = assemblies.SelectMany(a => a.GetTypes()).Where(t => t.Name.EndsWith("Repository") && t.IsClass);
            //foreach (Type type in types)
            //{
            //    Console.WriteLine($"Repository Registered: {type.Name}");
            //}
            //#endregion
        }
    }
}
