﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ResourceAPI;
using ResourceAPI.ApiServices;
using ResourceAPI.Controllers;

namespace ResourceAPIBenchmark
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var summary = BenchmarkRunner
                .Run<ResourceBenchmark>();
        }
    }

    public class ResourceBenchmark
    {
        private readonly IAuthorService _authorService;
        private readonly SqlContext _context;
        private readonly ProblemService _problemService;

        private ProblemsController _controller;

        public ResourceBenchmark()

        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true);

            var configuration = builder.Build();


            var options = new DbContextOptionsBuilder().UseMySQL(configuration.GetConnectionString("Default"));
            _context = new SqlContext(options.Options);
            _problemService = new ProblemService(_context);
            _authorService = new AuthorService(_context);
            _controller = new ProblemsController(null, _context, _problemService, _authorService);
        }

        public void Dispose()
        {
            _context.Dispose();
        }


        [Benchmark]
        public void GetProblem()
        {
            for (var j = 0; j < 100; j++)
            for (var i = 1; i < 10; i++)
                _problemService.ProblemById(i);
        }

        //[Benchmark] public void GetProblemStandard()
        //{
        //    for (int id = 1; id < 10; id++)
        //    {
        //        var problem = _context.Problems
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