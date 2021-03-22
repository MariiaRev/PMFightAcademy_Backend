using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using PMFightAcademy.Admin.DataBase;
using PMFightAcademy.Admin.Services;
using PMFightAcademy.Admin.Services.ServiceInterfaces;

namespace PMFightAcademy.Admin
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PMFightAcademy.Admin", Version = "v1" });
                
                var filePath = Path.Combine(AppContext.BaseDirectory, "PMFightAcademy_Admin.xml");
                c.IncludeXmlComments(filePath);
                c.EnableAnnotations();
            });
            services.AddTransient<SlotService>();

            services.AddDbContext<AdminContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("AdminContext")), ServiceLifetime.Transient);

            services.AddTransient<IBookingService,BookingService>();
            services.AddTransient<ICoachService,CoachService>();
            services.AddTransient<IServiceService,ServiceService>();
            services.AddTransient<IClientService,ClientService>();
            services.AddTransient<IQualificationService,QualificationService>();
            services.AddTransient<ISlotService,SlotService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PMFightAcademy.Admin v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
