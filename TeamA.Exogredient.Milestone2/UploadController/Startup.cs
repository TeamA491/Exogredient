using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeamA.Exogredient.Managers;
using TeamA.Exogredient.Services;
using TeamA.Exogredient.DAL;
using TeamA.Exogredient.AppConstants;

namespace UploadController
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
            services.AddControllers();
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin()
                                                             .AllowAnyHeader()
                                                             .AllowAnyMethod());
            });

            // Adding managers.
            services.AddTransient<RegistrationManager>();
            services.AddTransient<LoggingManager>();
            services.AddTransient<UploadManager>();

            // Adding services.
            services.AddTransient<MaskingService>();
            services.AddTransient<DataStoreLoggingService>();
            services.AddTransient<FlatFileLoggingService>();
            services.AddTransient<GoogleImageAnalysisService>();
            services.AddTransient<StoreService>();
            services.AddTransient<UploadService>();
            services.AddTransient<UserManagementService>();

            // Adding DAL.
            services.AddSingleton(new MapDAO(Constants.MapSQLConnection));
            services.AddSingleton(new LogDAO(Constants.NOSQLConnection));
            services.AddSingleton(new StoreDAO(Constants.SQLConnection));
            services.AddSingleton(new AnonymousUserDAO(Constants.SQLConnection));
            services.AddSingleton(new UploadDAO(Constants.SQLConnection));
            services.AddSingleton(new UserDAO(Constants.SQLConnection));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("AllowOrigin");
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
