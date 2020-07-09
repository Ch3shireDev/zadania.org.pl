using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ResourceAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //var connStr = Configuration.GetConnectionString("Default");
            //SqlContext.ConnectionString = connStr;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SqlContext>((serviceProvider, options) =>
            {
                //options.UseSqlite("Filename=sqlite.db");
                //options.EnableSensitiveDataLogging();
                options.UseMySQL(Configuration.GetConnectionString("Default"));
            });


            services.AddControllers();
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }
            ).AddJwtBearer(options =>
                {
                    options.Authority = "https://dev-f8t1k7iq.auth0.com/";
#if DEBUG
                    options.Audience = "http://localhost:5000";
#else
                    options.Audience = "https://zadania.org.pl";
#endif
                }
            );
            services.AddCors(o =>
                o.AddPolicy("MyPolicy", builder => { builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); }));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<SqlContext>();
#if DEBUG
                //context.Database.EnsureDeleted();
#endif
                context.Database.EnsureCreated();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}