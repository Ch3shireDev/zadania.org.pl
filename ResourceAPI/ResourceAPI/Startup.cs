using System;
using System.IO;
using System.Reflection;
using CategoryLibrary;
using CommonLibrary;
using ExerciseLibrary;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ProblemLibrary;
using QuizLibrary;
using ResourceAPI.ApiServices;

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

            services.AddScoped<IProblemDbContext>(provider => provider.GetService<SqlContext>());
            services.AddScoped<ICategoryDbContext>(provider => provider.GetService<SqlContext>());
            services.AddScoped<IQuizDbContext>(provider => provider.GetService<SqlContext>());
            services.AddScoped<IExerciseDbContext>(provider => provider.GetService<SqlContext>());


            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IProblemService, ProblemService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddScoped<IExerciseService, ExerciseService>();
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddControllers();


            services.AddCors(o =>
                o.AddDefaultPolicy(builder => { builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); }));

            ConfigureAuthentication(services);

            if (Environment.IsEnvironment("tests")) services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();

            services.AddRouting(options => options.LowercaseUrls = true);

            var problemLibraryAssembly = Assembly.Load("ProblemLibrary");
            services.AddMvc().AddApplicationPart(problemLibraryAssembly).AddControllersAsServices();

            var QuizLibraryAssembly = Assembly.Load("QuizLibrary");
            services.AddMvc().AddApplicationPart(QuizLibraryAssembly).AddControllersAsServices();

            var exerciseLibraryAssembly = Assembly.Load("ExerciseLibrary");
            services.AddMvc().AddApplicationPart(exerciseLibraryAssembly).AddControllersAsServices();

            var categoryLibraryAssembly = Assembly.Load("CategoryLibrary");
            services.AddMvc().AddApplicationPart(categoryLibraryAssembly).AddControllersAsServices();

            services.AddSwaggerGen(c =>
            {
                //c.SwaggerGeneratorOptions.DescribeAllParametersInCamelCase = false;


                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Zadania API",
                    Version = "v1",
                    Description = "API do strony [zadania.org.pl](https://zadania.org.pl)."
                });

                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "CategoryLibrary.xml"), true);
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "ProblemLibrary.xml"), true);
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "QuizLibrary.xml"), true);
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "CommonLibrary.xml"), true);
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "ExerciseLibrary.xml"), true);
            });
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
            app.UseStaticFiles();
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

            app.UseSwagger(c => { c.SerializeAsV2 = true; });
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zadania API"));

            app.UseRouting();
            app.UseResponseCaching();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}