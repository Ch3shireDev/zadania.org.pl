using System;
using System.IO;
using System.Reflection;
using AutoMapper;
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
                if (Environment.IsDevelopment())
                    options.UseInMemoryDatabase(guid).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                else options.UseMySQL(Configuration.GetConnectionString("Default"));
            });

            services.AddScoped<IAuthorDbContext>(provider => provider.GetService<SqlContext>());
            services.AddScoped<IProblemDbContext>(provider => provider.GetService<SqlContext>());
            services.AddScoped<ICategoryDbContext>(provider => provider.GetService<SqlContext>());
            services.AddScoped<IQuizDbContext>(provider => provider.GetService<SqlContext>());
            services.AddScoped<IExerciseDbContext>(provider => provider.GetService<SqlContext>());
            services.AddScoped<IVoteDbContext>(provider => provider.GetService<SqlContext>());

            if (Environment.IsDevelopment())
            {
                services.AddScoped<IAuthorService, MockAuthorService>();
                services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();
            }
            else
            {
                services.AddScoped<IAuthorService, AuthorService>();
            }

            services.AddScoped<IProblemService, ProblemService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddScoped<IExerciseService, ExerciseService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IVoteService, VoteService>();

            services.AddControllers();


            services.AddCors(o =>
                o.AddDefaultPolicy(builder => { builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); }));

            ConfigureAuthentication(services);

            services.AddRouting(options => options.LowercaseUrls = true);

            var commonLibraryAssembly = Assembly.Load("CommonLibrary");
            var problemLibraryAssembly = Assembly.Load("ProblemLibrary");
            var quizLibraryAssembly = Assembly.Load("QuizLibrary");
            var exerciseLibraryAssembly = Assembly.Load("ExerciseLibrary");
            var categoryLibraryAssembly = Assembly.Load("CategoryLibrary");
            var voteLibraryAssembly = Assembly.Load("VoteLibrary");

            services.AddMvc().AddApplicationPart(commonLibraryAssembly).AddControllersAsServices();
            services.AddMvc().AddApplicationPart(problemLibraryAssembly).AddControllersAsServices();
            services.AddMvc().AddApplicationPart(quizLibraryAssembly).AddControllersAsServices();
            services.AddMvc().AddApplicationPart(exerciseLibraryAssembly).AddControllersAsServices();
            services.AddMvc().AddApplicationPart(categoryLibraryAssembly).AddControllersAsServices();
            services.AddMvc().AddApplicationPart(voteLibraryAssembly).AddControllersAsServices();

            services.AddAutoMapper(typeof(Startup));

            services.AddSwaggerGen(c =>
            {
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
                context.Initialize();
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