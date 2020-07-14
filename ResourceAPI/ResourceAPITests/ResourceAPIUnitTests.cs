using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ResourceAPI;
using ResourceAPI.Controllers;
using ResourceAPI.Services;
using Xunit;
using Xunit.Abstractions;

namespace ResourceApiTests
{
    public abstract class TestsBase : IDisposable
    {
        protected TestsBase()
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
    }


    public class ResourceApiUnitTests : TestsBase
    {
        public ResourceApiUnitTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private readonly ITestOutputHelper _testOutputHelper;

        [Fact]
        public void GetProblems()
        {
            //if (!(Controller.Browse() is OkObjectResult problems)) return;
            //var value = problems.Value;
            //var list = value?.GetType().GetProperty("problemLinks")?.GetValue(value, null) as IEnumerable<string>;
            //foreach (var link in list)
            //{
            //    var problemId = Convert.ToInt32(link.Split('/').Last());
            //    var problem = ProblemService.GetById(problemId);
            //    foreach (var answerLink in problem.AnswerLinks)
            //    {
            //        var answerId = Convert.ToInt32(answerLink.Split('/').Last());
            //        var answer = ProblemService.GetAnswerById(problemId, answerId);
            //    }
            //}
        }
        //[Theory]
        //[InlineData(1, 2, 3)]
        //[InlineData(-4, -6, -10)]
        //[InlineData(-2, 2, 0)]
        //public void ManySums(int a, int b, int c)
        //{
        //    Assert.Equal(c, Sum(a, b));
        //}


        //private int Sum(int a, int b)
        //{
        //    return 2 * a + (b - a);
        //}

        //[Fact]
        //public void SingleSum()
        //{
        //    Assert.Equal(4, 2 + 2);
        //}

        [Fact]
        public void TestProblemRequest()
        {
            for (var i = 1; i < 200; i++)
                //Controller.Get(i).ExecuteResult(new ActionContext());
                ProblemService.GetById(i);
        }
    }
}