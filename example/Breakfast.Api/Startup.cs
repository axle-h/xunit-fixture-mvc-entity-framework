using System;
using Breakfast.Api.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Breakfast.Api
{
    public class Startup
    {
        private readonly string _mysqlConnectionString;

        public Startup(IConfiguration configuration)
        {
            _mysqlConnectionString = configuration.GetConnectionString("mysql");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
                                   {
                                       c.SwaggerDoc("v1", new OpenApiInfo { Title = "Breakfast API", Version = "v1" });
                                   });

            services.AddDbContext<BreakfastContext>(o => o.UseMySql(_mysqlConnectionString, mo => mo.ServerVersion(new Version(5, 7, 21), ServerType.MySql)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Breakfast API V1"));

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
