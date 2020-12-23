using Autofac;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using ProductDb.Common.Cache;
using ProductDb.Repositories.BaseRepositories;
using ProductDb.Repositories.UnitOfWorks;
using ProductDb.Services.ImportServices.SupplierIoc;

namespace ProductDb.Services.Ioc
{
    public class IocModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Cache Manager Inject
            builder.RegisterType<CacheManager>().As<ICacheManager>().InstancePerLifetimeScope();
            // Redis Cache Inject
            builder.RegisterType<RedisCache>().As<IRedisCache>().InstancePerLifetimeScope();

            builder.RegisterType(typeof(UnitOfWork)).As(typeof(IUnitOfWork)).AsSelf().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(GenericRepository<>)).As(typeof(IGenericRepository<>)).AsSelf().InstancePerLifetimeScope();

            builder.RegisterType(typeof(UnitOfWorkLogo)).As(typeof(IUnitOfWorkLogo)).AsSelf().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(GenericRepositoryLogo<>)).As(typeof(IGenericRepositoryLogo<>)).AsSelf().InstancePerLifetimeScope();

            builder.RegisterType(typeof(UnitOfWorkNop)).As(typeof(IUnitOfWorkNop)).AsSelf().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(GenericRepositoryNop<>)).As(typeof(IGenericRepositoryNop<>)).AsSelf().InstancePerLifetimeScope();

            builder.RegisterType(typeof(UnitOfWorkOnnet)).As(typeof(IUnitOfWorkOnnet)).AsSelf().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(GenericRepositoryOnnet<>)).As(typeof(IGenericRepositoryOnnet<>)).AsSelf().InstancePerLifetimeScope();

            builder.RegisterType(typeof(Mapper)).As(typeof(IMapper)).AsSelf().InstancePerLifetimeScope();
            builder.RegisterType(typeof(HttpContextAccessor)).As(typeof(IHttpContextAccessor)).AsSelf().InstancePerLifetimeScope();

            builder.RegisterModule(new SupplierIocModule());

            builder.RegisterAssemblyTypes(System.Reflection.Assembly.Load("ProductDb.Mapping"))
                .Where(t => t.Name.EndsWith("Configuration"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(System.Reflection.Assembly.Load("ProductDb.Logging"))
              .Where(t => t.Name.EndsWith("Manager"))
              .AsImplementedInterfaces()
              .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(System.Reflection.Assembly.Load("ProductDb.Services"))
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
