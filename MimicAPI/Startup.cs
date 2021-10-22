﻿using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.Repositories;
using MimicAPI.Repositories.Contracts;

namespace MimicAPI
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region AutoMapper-Configuracao
            
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfileDTO());
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            #endregion

            services.AddDbContext<MimicContext>(opt =>
            {
                opt.UseSqlite(@"Data Source=Database\Mimic.db");
            });

            services.AddMvc();
            services.AddScoped<IPalavraRepository, PalavraRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();
            app.UseMvc();
        }
    }
}
