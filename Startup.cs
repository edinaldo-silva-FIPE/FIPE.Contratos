using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;

namespace ApiFipe
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
            FIPEContratosContext.ConnectionString = Configuration.GetConnectionString("DefaultConnection");

            //EGS 30.05.2020 - Diferenciando ambiente HML e PRD
            var IsProduction = Configuration.GetConnectionString("EnvironmentIsProduction");
            if (string.IsNullOrEmpty(IsProduction)) {
                FIPEContratosContext.EnvironmentIsProduction = true; 
            } else { 
                FIPEContratosContext.EnvironmentIsProduction = Convert.ToBoolean(IsProduction); 
            }


            bEmail.ConnnectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors();
            services.AddScoped<AutenticacaoActionFilter>();

            services.Configure<FormOptions>(opt =>
            {
                //opt.ValueLengthLimit = int.MaxValue;
                opt.MultipartBodyLengthLimit = int.MaxValue;
                //opt.MultipartHeadersLengthLimit = int.MaxValue;
            });
            Clock.AtualizaEntregaveisParcelas.Run().GetAwaiter().GetResult();            
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

            var supportedCultures = new[] { new CultureInfo("pt-BR") };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseHttpsRedirection();
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            app.UseMvc();
        }
    }
}
