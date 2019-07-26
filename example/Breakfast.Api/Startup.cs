using System;
using Breakfast.Api.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Swashbuckle.AspNetCore.Swagger;

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
            services.AddMvc();
            services.AddSwaggerGen(c =>
                                   {
                                       c.SwaggerDoc("v1", new Info { Title = "Breakfast API", Version = "v1" });
                                   });

            services.AddDbContext<BreakfastContext>(o => o.UseMySql(_mysqlConnectionString, mo => mo.ServerVersion(new Version(5, 7, 21), ServerType.MySql)));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Breakfast API V1"));


            app.UseMvc();
        }
    }
}
