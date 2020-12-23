using ApiClient.HttpClient;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Admin.Resources;
using ProductDb.Data.BiggBrandsDb;
using ProductDb.Data.LogoDb;
using ProductDb.Data.NopDb;
using ProductDb.Data.OnnetDb;
using ProductDb.Services.Ioc;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ProductDb.Admin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 1024 * 1024 * 100;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(CommonResource));
                })
                .AddViewLocalization(LanguageViewLocationExpanderFormat.SubFolder)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

            services.AddMemoryCache();

            services.AddDbContext<BiggBrandsDbContext>(opts => opts.UseSqlServer(Configuration["ConnectionStrings:BiggBrandsDbConnection"]));
            services.AddDbContext<LogoDbContext>(opts => opts.UseSqlServer(Configuration["ConnectionStrings:LogoDbConnection"]));
            services.AddDbContext<NopDbContext>(opts => opts.UseSqlServer(Configuration["ConnectionStrings:NopDbConnection"]));
            services.AddDbContext<OnnetDbContext>(opts => opts.UseSqlServer(Configuration["ConnectionStrings:OnnetDbConnection"]));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            }).AddCookie(options =>
            {
                options.LoginPath = "/auth/login";
                options.LogoutPath = "/auth/logout";
                options.AccessDeniedPath = "/auth/access-denied";
            });

            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("lang", typeof(LanguageRouteConstraint));
            });

            services.AddTransient<CustomLocalizer>();
            services.AddTransient<IApiRepo, ApiRepo>();

            // PMS.API Api
            services.AddTransient<PMS.Common.IApiRepo, PMS.Common.ApiRepo>();
            services.AddTransient<Areas.PMS.Services.IApiLogService, Areas.PMS.Services.ApiLogService>();
            services.AddTransient<Areas.PMS.Services.IApiExcelService, Areas.PMS.Services.ApiExcelService>();
            services.AddTransient<Areas.PMS.Services.IOrderService, Areas.PMS.Services.OrderService>();
            services.AddTransient<Common.Cache.ICacheManager, Common.Cache.CacheManager>();

            // setup the Autofac container
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
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            LocalizationPipeline.ConfigureOptions(options.Value);
            app.UseRequestLocalization(options.Value);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                   name: "areas",
                   template: "{area:exists}/{controller=order}/{action=Index}/{id?}"
                 );
                routes.MapRoute(
                    name: "LocalizedDefault",
                    template: "{lang:lang}/{controller=Home}/{action=Index}/{id?}"
                );
                routes.MapRoute(
                   name: "default",
                   template: "{*catchall}",
                   defaults: new { controller = "Home", action = "RedirectToDefaultLanguage", lang = "en" });

            });

        }
    }
}
