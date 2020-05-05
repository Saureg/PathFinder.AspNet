using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using PathFinder.Data;
using PathFinder.Data.Interfaces;
using PathFinder.Data.Repository;

namespace PathFinder
{
    public class Startup
    {
        private readonly IConfigurationRoot _confString;

        public Startup(IHostEnvironment hostEnv)
        {
            _confString = new ConfigurationBuilder().SetBasePath(hostEnv.ContentRootPath).AddJsonFile("dbSettings.json")
                .Build();
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_confString.GetConnectionString("DefaultConnection")));
            
            services.AddTransient<IAllRaces, RaceRepository>();
            services.AddTransient<ICharacter, CharacterRepository>();
            services.AddTransient<IAllClasses, CharClassRepository>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddMvc(option =>
                option.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "Обязательное поле"));

            services.AddMemoryCache();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseSession();

            
            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}");
                //routes.MapRoute(name: "default", template: "{controller=Race}/{action=List}");
            });
            
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var content = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                DbObjects.Initial(content);   
            }
        }
    }
}