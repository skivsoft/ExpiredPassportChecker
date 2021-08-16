using ExpiredPassportChecker.Batches.UpdateExpiredPassports.Context;
using ExpiredPassportChecker.Batches.UpdateExpiredPassports.Processors;
using ExpiredPassportChecker.Helpers;
using ExpiredPassportChecker.HostedServices;
using ExpiredPassportChecker.Settings;
using FileFormat.PassportData;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ExpiredPassportChecker
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ExpiredPassportChecker", Version = "v1" });
            });
            services.AddOptions<MainSettings>();
            services.Configure<MainSettings>(Configuration.GetSection(nameof(MainSettings)));

            services.AddMediatR(typeof(Startup));

            services.AddSingleton<InMemoryContainer<PassportDataStorage>>();

            var settings = Configuration.GetSection(nameof(MainSettings)).Get<MainSettings>();

            if (settings.EnabledSteps.DownloadBzip2)
            {
                services.AddTransient<IRequestHandler<ExpiredPassportsContext>, DownloadBzip2>();
            }

            if (settings.EnabledSteps.UnpackFromBzip2)
            {
                services.AddTransient<IRequestHandler<ExpiredPassportsContext>, UnpackFromBzip2>();
            }

            if (settings.EnabledSteps.PackCsvToPassportData)
            {
                services.AddTransient<IRequestHandler<ExpiredPassportsContext>, PackCsvToPassportData>();
            }
            
            if (settings.EnabledSteps.RepackBzip2ToPassportData)
            {
                services.AddTransient<IRequestHandler<ExpiredPassportsContext>, RepackBzip2ToPassportData>();
            }

            services.AddTransient<IRequestHandler<ExpiredPassportsContext>, SavePassportData>();
            
            services.AddHostedService<UpdateExpiredPassportsHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExpiredPassportChecker v1"));
            }

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
