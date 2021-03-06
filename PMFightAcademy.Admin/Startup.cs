using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PMFightAcademy.Dal.DataBase;
using PMFightAcademy.Admin.Filters;
using PMFightAcademy.Admin.Services;
using PMFightAcademy.Admin.Services.ServiceInterfaces;
using System;
using System.IO;

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
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ExceptionFilter));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PMFightAcademy.Admin", Version = "v1" });

                var filePath = Path.Combine(AppContext.BaseDirectory, "PMFightAcademy_Admin.xml");
                c.IncludeXmlComments(filePath);
                c.EnableAnnotations();
            });

            services.AddCors();

            services.AddDbContext<ApplicationContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("AdminContext")), ServiceLifetime.Transient);

            services.AddTransient<IBookingService, BookingService>();
            services.AddTransient<ICoachService, CoachService>();
            services.AddTransient<IServiceService, ServiceService>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IQualificationService, QualificationService>();
            services.AddTransient<ISlotService, SlotService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PMFightAcademy.Admin v1"));

            app.UseCors(builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

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
