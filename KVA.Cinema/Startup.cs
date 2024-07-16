using KVA.Cinema.Entities;
using KVA.Cinema.Models;
using KVA.Cinema.Services;
using KVA.Cinema.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace KVA.Cinema
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<CinemaContext>();

            services.AddIdentityCore<User>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<CinemaContext>()
                    .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider);

            string connection = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<CinemaContext>(options => options.UseLazyLoadingProxies().UseSqlServer(connection));

            services.AddHttpContextAccessor();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped(provider =>
            {
                var actionContext = provider.GetService<IActionContextAccessor>().ActionContext;
                var factory = provider.GetService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            services.AddTransient<EmailSender>()
                    .AddTransient<UserService>()
                    .AddTransient<VideoService>()
                    .AddTransient<CountryService>()
                    .AddTransient<DirectorService>()
                    .AddTransient<GenreService>()
                    .AddTransient<SubscriptionLevelService>()
                    .AddTransient<SubscriptionService>()
                    .AddTransient<PegiService>()
                    .AddTransient<LanguageService>()
                    .AddTransient<TagService>()
                    .AddTransient<CacheManager>();

            services.AddControllersWithViews();
            services.AddMemoryCache();

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection()
               .UseStaticFiles()
               .UseAuthorization()
               .UseRouting()
               .UseAuthentication()
               .UseEndpoints(endpoints =>
               {
                   endpoints.MapControllerRoute(
                       name: "default",
                       pattern: "{controller=Home}/{action=Index}/{id?}");
               });
        }
    }
}
