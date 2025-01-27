﻿using AutoMapper;
using BenchmarkDotNet.Attributes;
using CategoryLibrary;
using CommonLibrary;
using FileDataLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProblemLibrary;
using ResourceAPI;

namespace ResourceAPIBenchmark
{
    public class ResourceBenchmark
    {
        private readonly IAuthorService _authorService;
        private readonly ICategoryService _categoryService;
        private readonly SqlContext _context;

        private readonly ProblemsController _controller;
        private readonly IProblemService _problemService;

        public ResourceBenchmark()

        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true);

            var configuration = builder.Build();

            var conf = new MapperConfiguration(c => { });
            var mapper = new Mapper(conf);

            var options = new DbContextOptionsBuilder().UseMySQL(configuration.GetConnectionString("Default"));
            _context = new SqlContext(options.Options);
            var fileDataService = new FileDataService(_context);
            _categoryService = new CategoryService(_context);
            _problemService = new ProblemService(_context, fileDataService);
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
                _problemService.Get(i);
        }

        //[Benchmark] public void GetProblemStandard()
        //{
        //    for (int id = 1; id < 10; id++)
        //    {
        //        var problem = _context.Problems
        //            .Select(p => new Problem
        //            {
        //                Id = p.Id,
        //                Name = p.Name,
        //                Content = p.Content,
        //             })
        //            .FirstOrDefault(p => p.Id == id);
        //    }
        //}
    }
}