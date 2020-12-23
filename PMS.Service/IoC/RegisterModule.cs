using Autofac;
using PMS.Data.Repository;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Module = Autofac.Module;

namespace PMS.Service.IoC
{
    public class RegisterModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(Repository<>))
                        .As(typeof(IRepository<>));

            builder.RegisterAssemblyTypes(Assembly.Load("PMS.Mapping")).Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(Assembly.Load("PMS.LogoService")).Where(t => t.Name.EndsWith("Service"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(Assembly.Load("PMS.Service")).Where(t => t.Name.EndsWith("Service"))
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();
        }
    }
}
