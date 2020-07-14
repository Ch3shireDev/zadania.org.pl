using System;
using System.Linq;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ResourceAPI;
using ResourceAPI.Controllers;
using ResourceAPI.Models;
using ResourceAPI.Services;

namespace ResourceAPIBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner
                .Run<ResourceBenchmark>();
        }
    }

    public class ResourceBenchmark
    {
        public ResourceBenchmark()
        
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true);

            var configuration = builder.Build();


            var options = new DbContextOptionsBuilder().UseMySQL(configuration.GetConnectionString("Default"));
            Context = new SqlContext(options.Options);
            ProblemService = new ProblemService(Context);
            Controller = new ProblemsController(null, Context, ProblemService);
        }

        public ProblemsController Controller { get; }
        private SqlContext Context { get; }
        public ProblemService ProblemService { get; }

        public void Dispose()
        {
            Context.Dispose();
        }


        [Benchmark]
        public void GetProblem()
        {
            for (int i = 1; i < 10000; i++)
                ProblemService.GetById(i);
        }

        //[Benchmark] public void GetProblemStandard()
        //{
        //    for (int id = 1; id < 10; id++)
        //    {
        //        var problem = Context.Problems
        //            .Select(p => new Problem
        //            {
        //                Id = p.Id,
        //                Title = p.Title,
        //                Content = p.Content,
        //             })
        //            .FirstOrDefault(p => p.Id == id);
        //    }
        //}
    }
}
