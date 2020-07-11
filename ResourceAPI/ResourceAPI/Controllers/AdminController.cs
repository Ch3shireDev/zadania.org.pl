using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResourceAPI.Models;

namespace ResourceAPI.Controllers
{
#if DEBUG
    [Route("api/v1/[controller]")]
    [ApiController]
#endif
    public class AdminController : ControllerBase
    {
        public AdminController(ILogger<ProblemsController> logger, SqlContext context)
        {
            Logger = logger;
            Context = context;
        }

        private ILogger<ProblemsController> Logger { get; }
        private SqlContext Context { get; }

        [HttpPost]
        [Route("upload/eko2")]
        public ActionResult PostEconomy()
        {
            var lines = System.IO.File.ReadAllLines("../../../WIT-Zajecia/semestr-2/Ekonomia 2/egzamin-1.md");
            var mdElement = new MdElement(lines);

            var author = AuthorsController.GetAuthor(HttpContext, Context);
            var test = new MultipleChoiceTest
            {
                Title = mdElement.Title,
                Content = mdElement.Content,
                Author = author,
                Questions = mdElement.Children[0].Children.Select(c => c.ToQuestion(author)).ToList()
            };

            Context.MultipleChoiceTests.Add(test);
            Context.SaveChanges();

            return StatusCode(200);
        }

        [HttpPost]
        [Route("upload")]
        public ActionResult PostFiles([FromQuery] string all = null)
        {
            var curr = Directory.GetCurrentDirectory();
            var filePath = Path.Join(curr, "../zadania.info/exercises.md");

            var author = Context.Authors.FirstOrDefault(auth => auth.Name == "zadania.info");
            if (author == null)
            {
                author = new Author {Name = "zadania.info"};
                Context.Authors.Add(author);
                Context.SaveChanges();
            }

            var text = System.IO.File.ReadAllText(filePath);
            var problems = GetProblemsFromMd(text, filePath);

            var i = 0;
            foreach (var problem in problems)
            {
                Context.AddProblem(problem, author, true);
                i++;
                if (i % 100 == 0)
                {
                    Context.SaveChanges();
                    Console.WriteLine(i);
                }

                //if (i > 100) break;
            }

            Context.SaveChanges();
            return StatusCode(200);
        }

        [HttpPost]
        [Route("cleanTags")]
        public ActionResult CleanTags()
        {
            var pt = Context.ProblemTags.Where(pt => pt.Problem == null || pt.Tag == null);
            Context.ProblemTags.RemoveRange(pt);
            Context.SaveChanges();

            var tags = Context.Tags.Where(t => t.ProblemTags.Count == 0);
            Context.Tags.RemoveRange(tags);
            Context.SaveChanges();
            return StatusCode(200);
        }


        [Route("upload2")]
        [HttpPost]
        public void LoadMd()
        {
            var author = Context.Authors.FirstOrDefault(a => a.Name == "Igor Nowicki");
            var problems = GetProblemsFromDirectory("../../WIT-Zajecia/semestr-2/OAK");
            var n = 1;
            foreach (var problem in problems)
            {
                problem.Title = $"Zadanie {n++}";
                problem.Tags = new List<Tag> {new Tag {Name = "OAK"}, new Tag {Name = "Informatyka"}};
                Context.AddProblem(problem, author, true);
            }

            Context.SaveChanges();
        }

        private IEnumerable<Problem> GetProblemsFromDirectory(string dir)
        {
            var files = Directory.GetFiles(dir);
            foreach (var file in files)
            {
                var text = System.IO.File.ReadAllText(file).Replace("\r", "");
                var problems = GetProblemsFromMd(text, dir);
                foreach (var problem in problems) yield return problem;
            }
        }

        private IEnumerable<Problem> GetProblemsFromMd(string text, string path = null)
        {
            var matches = Regex.Matches(text, @"### (?:Zad.*)[\s\n]*([^#]+)(?:### (?:Rozw.*)[\s\n]*([^#]+))?",
                RegexOptions.Multiline);
            var elements = matches.Select(m => new Element(m, path)).Select(e => e.GetProblem());

            foreach (var element in elements) yield return element;
        }

    }
}