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
            var connStr = Configuration.GetConnectionString("Default");
            DatabaseContext.ConnectionString = connStr;
            var context = new DatabaseContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlite().AddDbContext<DatabaseContext>(options =>
            {
                options.UseMySQL(Configuration.GetConnectionString("Default"));
            });


            services.AddControllers();
            //services.AddAuthentication(options =>
            //    {
            //        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    }
            //).AddJwtBearer(options =>
            //    {
            //        options.Authority = "https://dev-f8t1k7iq.auth0.com/";
            //        options.Audience = "https://zadania.org.pl";
            //    }
            //);
            services.AddCors(o =>
                o.AddPolicy("MyPolicy", builder => { builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); }));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseRouting();
            //app.UseAuthentication();
            //app.UseAuthorization();
            app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}