using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResourceAPI.ApiServices.Interfaces;
using ResourceAPI.Models.MultipleChoice;
using ResourceAPI.Models.Post;
using ResourceAPI.Models.Problem;
using ResourceAPI.Tools;

namespace ResourceAPI.Controllers
{
#if DEBUG
    [Route("api/v1/[controller]")]
    [ApiController]
#endif
    public class AdminController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly SqlContext _context;
        private readonly IProblemService _problemService;

        private ILogger<ProblemsController> _logger;

        public AdminController(ILogger<ProblemsController> logger, SqlContext context, IProblemService problemService,
            IAuthorService authorService)
        {
            _logger = logger;
            _context = context;
            _problemService = problemService;
            _authorService = authorService;
        }

        [HttpPost("upload/eko2")]
        public ActionResult PostEconomy()
        {
            var lines = System.IO.File.ReadAllLines("../../../WIT-Zajecia/semestr-2/Ekonomia 2/egzamin-1.md");
            var mdElement = new MdElement(lines);

            var author = _authorService.GetAuthor(1);
            var test = new MultipleChoiceTest
            {
                Name = mdElement.Title,
                Content = mdElement.Content,
                Author = author,
                Questions = mdElement.Children[0].Children.Select(c => c.ToQuestion(author)).ToList()
            };

            _context.MultipleChoiceTests.Add(test);
            _context.SaveChanges();

            return StatusCode(200);
        }

        [HttpPost("upload")]
        public ActionResult PostFiles([FromQuery] string all = null)
        {
            var curr = Directory.GetCurrentDirectory();
            var filePath = Path.Join(curr, "../zadania.info/exercises.md");

            var author = _context.Authors.FirstOrDefault(auth => auth.Name == "zadania.info");
            if (author == null)
            {
                author = new Author {Name = "zadania.info"};
                _context.Authors.Add(author);
                _context.SaveChanges();
            }

            var text = System.IO.File.ReadAllText(filePath);
            var problems = GetProblemsFromMd(text, filePath);

            var i = 0;
            foreach (var problem in problems)
            {
                _problemService.Create(1, problem);
                i++;
                if (i % 100 == 0)
                {
                    _context.SaveChanges();
                    Console.WriteLine(i);
                }

                //if (i > 100) break;
            }

            _context.SaveChanges();
            return StatusCode(200);
        }

        [HttpPost("cleanTags")]
        public ActionResult CleanTags()
        {
            var pt = _context.ProblemTags.Where(pt => pt.Problem == null || pt.Tag == null);
            _context.ProblemTags.RemoveRange(pt);
            _context.SaveChanges();

            var tags = _context.Tags.Where(t => t.ProblemTags.Count == 0);
            _context.Tags.RemoveRange(tags);
            _context.SaveChanges();
            return StatusCode(200);
        }


        [Route("upload2")]
        [HttpPost]
        public void LoadMd()
        {
            var author = _context.Authors.FirstOrDefault(a => a.Name == "Igor Nowicki");
            var problems = GetProblemsFromDirectory("../../WIT-Zajecia/semestr-2/OAK");
            var n = 1;
            foreach (var problem in problems)
            {
                problem.Name = $"Zadanie {n++}";
                problem.Tags = new List<Tag> {new Tag {Name = "OAK"}, new Tag {Name = "Informatyka"}};
                _problemService.Create(1, problem);
            }

            _context.SaveChanges();
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