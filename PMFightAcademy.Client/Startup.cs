using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PMFightAcademy.Client.Authorization;
using PMFightAcademy.Dal.DataBase;
using PMFightAcademy.Client.Filters;
using PMFightAcademy.Client.Services;

namespace PMFightAcademy.Client
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
            ////add httpClient
            services.AddHttpClient();

            // add services
            services.AddTransient<ICoachesService, CoachesService>();
            services.AddTransient<IBookingService, BookingService>();
            services.AddTransient<IClientsService, ClientsService>();

            ////add authentication by jwt
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.Audience,
                        ValidateLifetime = true,
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                    };
                });

            //add authorization by jwt
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddCors();

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ExceptionFilter));
            });

            services.AddDbContext<ApplicationContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("ClientContext")), ServiceLifetime.Transient);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PMFightAcademy.Client", Version = "v1" });
                
                var filePath = Path.Combine(AppContext.BaseDirectory, "PMFightAcademy_Client.xml");
                c.IncludeXmlComments(filePath);
                c.EnableAnnotations();

                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "Bearer",
                                    Type = ReferenceType.SecurityScheme
                                },
                            },
                            new string[0]
                        }
                    });

                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.ApiKey,
                        In = ParameterLocation.Header,
                        Scheme = "Bearer",
                        Name = "Authorization",
                        Description = "JWT token",
                        BearerFormat = "JWT"
                    });
            });
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
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
