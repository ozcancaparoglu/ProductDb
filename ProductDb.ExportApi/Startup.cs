using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductDb.Common.HttpClient;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Data.NopDb;
using ProductDb.ExportApi.Services;
using ProductDb.ExportApi.Services.CaFactories;
using ProductDb.ExportApi.Services.CaServices;
using ProductDb.Logging;
using ProductDb.Services.Ioc;

namespace ProductDb.ExportApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<BiggBrandsDbContext>(opts => opts.UseSqlServer(Configuration["ConnectionStrings:ServerBiggBrandsDbConnection"]));
            services.AddDbContext<NopDbContext>(opts => opts.UseSqlServer(Configuration["ConnectionStrings:NopDbConnection"]));

            // Redis Cache Configuration
            services.AddDistributedRedisCache(option => {
                option.Configuration = Configuration["RedisConfiguration:Host"];
                option.InstanceName = Configuration["RedisConfiguration:RedisDB"];
            });

            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterType<IntegrationService>().As<IIntegrationService>().InstancePerLifetimeScope();

            // CA Service Injected
            builder.RegisterType<CaService>().As<ICaService>().InstancePerLifetimeScope();
            builder.RegisterType<CaFactory>().As<ICaFactory>().InstancePerLifetimeScope();
            // Logginng Injected
            builder.RegisterType<LoggerManager>().As<ILoggerManager>().InstancePerLifetimeScope();
            // Api Repo Injected
            builder.RegisterType<ApiRepo>().As<IApiRepo>().InstancePerLifetimeScope();

            builder.RegisterModule(new IocModule());
            var container = builder.Build();
            // return the IServiceProvider implementation
            return new AutofacServiceProvider(container);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
