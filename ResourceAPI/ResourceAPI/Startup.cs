using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ResourceAPI.ApiServices;
using ResourceAPI.ApiServices.Interfaces;

namespace ResourceAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            var guid = Guid.NewGuid().ToString();
            services.AddResponseCompression();
            services.AddDbContext<SqlContext>((serviceProvider, options) =>
            {
                if (Environment.IsEnvironment("tests"))
                    options.UseInMemoryDatabase(guid).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                else if (Environment.IsDevelopment()) options.UseSqlite("Filename=sqlite.db");
                //options.UseMySQL(Configuration.GetConnectionString("Local"));
                else options.UseMySQL(Configuration.GetConnectionString("Default"));
            });


            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IProblemService, ProblemService>();
            services.AddScoped<IMultipleChoiceService, MultipleChoiceService>();
            services.AddScoped<IExerciseService, ExerciseService>();
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddControllers();

            services.AddCors(o =>
                o.AddDefaultPolicy(builder => { builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); }));

            ConfigureAuthentication(services);

            if (Environment.IsEnvironment("tests")) services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();
        }

        protected virtual void ConfigureAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }
            ).AddJwtBearer(options =>
                {
                    options.Authority = "https://dev-f8t1k7iq.auth0.com/";
                    options.Audience = Environment.IsProduction() ? "https://zadania.org.pl" : "http://localhost:5000";
                }
            );
        }

        public virtual void Configure(IApplicationBuilder app)
        {
            app.UseResponseCompression();
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<SqlContext>();
                if (Environment.IsEnvironment("tests")) context.Database.EnsureDeleted();
#if DEBUG
                if (Environment.IsDevelopment()) context.Database.EnsureDeleted();
#endif
                context.Database.EnsureCreated();
            }

            app.UseRouting();
            app.UseResponseCaching();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}