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
#if DEBUG
        [HttpPost]
        [Route("upload")]
#endif
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

                if (i > 100) break;
            }

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


#if DEBUG
        [Route("upload2")]
        [HttpPost]
#endif
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


        private class Element
        {
            public Element(Match match, string path)
            {
                ContentProblem = match.Groups[1].Value;
                ContentSolution = match.Groups[2].Value;

                var dir = Path.GetDirectoryName(path);

                ImagesProblem = Regex.Matches(ContentProblem, @"!\[\w*\]\((.*?)\)", RegexOptions.Multiline)
                    .Select(m => new FileData(m.Groups[1].Value, dir)).ToList();

                ImagesSolution = Regex.Matches(ContentSolution, @"!\[\w*\]\((.*?)\)", RegexOptions.Multiline)
                    .Select(m => new FileData(m.Groups[1].Value, dir)).ToList();

                var tagMatches = Regex.Match(ContentProblem, @"Tagi: (.*)");

                if (tagMatches.Success)
                {
                    var tags = tagMatches.Groups[1].Value.Split(";").Select(tag => tag.Trim())
                        .Where(tag => !string.IsNullOrWhiteSpace(tag))
                        .Select(tag => new Tag {Name = tag.Replace("...", "")})
                        .Select(tag => new Tag {Name = tag.Name, Url = tag.GenerateUrl()});

                    var dict = new Dictionary<string, Tag>();
                    foreach (var tag in tags)
                    {
                        if (dict.ContainsKey(tag.Url)) continue;
                        dict.Add(tag.Url, tag);
                    }

                    Tags = dict.Select(p => p.Value).ToList();

                    ContentProblem = ContentProblem.Replace(tagMatches.Value, "").Trim();
                }

                ContentProblem = ContentProblem.Replace("\r", "");
                ContentSolution = ContentSolution.Replace("\r", "");

                ContentProblem = Regex.Replace(ContentProblem, @"\(\./images/\d+/(\d+.gif)\)", @"($1)");
                ContentSolution = Regex.Replace(ContentSolution, @"\(\./images/\d+/(\d+.gif)\)", @"($1)");
            }

            public string ContentProblem { get; }
            public string ContentSolution { get; }

            public IEnumerable<Tag> Tags { get; }
            public ICollection<FileData> ImagesProblem { get; }
            public ICollection<FileData> ImagesSolution { get; }

            public Problem GetProblem()
            {
                var problem = new Problem
                {
                    Content = ContentProblem,
                    Tags = Tags,
                    FileData = ImagesProblem
                };

                if (!string.IsNullOrWhiteSpace(ContentSolution))
                    problem.Answers = new[]
                    {
                        new Answer
                        {
                            Content = ContentSolution,
                            FileData = ImagesSolution,
                            IsApproved = true
                        }
                    };


                if (string.IsNullOrWhiteSpace(problem.Title) && problem.Tags != null && problem.Tags.Any())
                    problem.Title = problem.Tags.Last().Name;

                return problem;
            }
        }
    }
}