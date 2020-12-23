using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiClient.HttpClient;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ProductDb.Common.Helpers.JwtHelper;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Services.Ioc;
using Swashbuckle.AspNetCore.Swagger;
using ProductDb.Common.Cache;
using ProductDb.Data.NopDb;

namespace ProductDb.Api.Export
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

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,

                      ValidIssuer = "aristo-it",
                      ValidAudience = "aristo-it",
                      IssuerSigningKey = JwtSecurityKey.Create("hurriyet-RC5qGgO1z0-7orEMKl2D6")
                  };

                  options.Events = new JwtBearerEvents
                  {
                      OnAuthenticationFailed = context =>
                      {
                          Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                          return Task.CompletedTask;
                      },
                      OnTokenValidated = context =>
                      {
                          Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                          return Task.CompletedTask;
                      }
                  };
              });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("ProductDbApi", new Info
                {
                    Title = "Swagger on ASP.NET Core",
                    Version = "1.0.0",
                    Description = "Try Swagger on (ASP.NET Core 2.2)",
                    Contact = new Contact()
                    {
                        Name = "Swagger Implementation ProductDb Api",
                        Url = "https://localhost:44347/",
                        Email = "bkıvcak@aristo.com.tr"
                    },
                    TermsOfService = "http://swagger.io/terms/"
                });
            });

            services.AddDbContext<BiggBrandsDbContext>(opts => opts.UseSqlServer(Configuration["ConnectionStrings:ServerBiggBrandsDbConnection"]));
            services.AddDbContext<NopDbContext>(opts => opts.UseSqlServer(Configuration["ConnectionStrings:NopDbConnection"]));
            services.AddTransient<IApiRepo, ApiRepo>();
            services.AddTransient<ICacheManager, CacheManager>();

            var builder = new ContainerBuilder();
            builder.Populate(services);
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
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseSwagger()
            .UseSwaggerUI(c =>
            {
                //TODO: Either use the SwaggerGen generated Swagger contract (generated from C# classes)
                c.SwaggerEndpoint("/swagger/ProductDbApi/swagger.json", "Swagger Test .Net Core");

                //TODO: Or alternatively use the original Swagger contract that's included in the static files
                // c.SwaggerEndpoint("/swagger-original.json", "Swagger Petstore Original");
            });

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
