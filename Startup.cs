using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement
{
    public class Startup
    {

        private IConfiguration _config;

        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddRazorPages();
            services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));
            services.AddMvc(x => x.EnableEndpointRouting = false);
            services.AddScoped<IEmployeeRepository, SqlEmployeeRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseMvcWithDefaultRoute();

            app.UseMvc(routes => {
                routes.MapRoute("default", "{Controller=Home}/{action=Index}/{id?}");
                        });

            app.UseRouting();

            //app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages();
            //});

        //    app.Use(async (context, next) =>
        //    {
         //       //await context.Response.WriteAsync(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
         //       await context.Response.WriteAsync(Configuration["MyKey"]);
         //       logger.LogInformation("MW1: Incoming Request");
          //      await next();
          //      logger.LogInformation("MW1: Outgoing Request");
          //  });

     //       app.Run(async (context) =>
     //       {
      //          //await context.Response.WriteAsync(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
      //          await context.Response.WriteAsync("Hellow from 2nd middleware");
      //          logger.LogInformation("MW2: Handled with Response");
      //      });

        }
    }
}
