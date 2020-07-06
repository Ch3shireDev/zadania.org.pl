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
                //Context.Problems.Add(problem);
                i++;
                if (i % 100 == 0)
                {
                    Context.SaveChanges();
                    Console.WriteLine(i);
                }

                if (i > 100) break;
            }


            return StatusCode(200);


            //var i = 0;
            //var n = dirs.Count;
            //foreach (var dir in dirs.OrderBy(a => Guid.NewGuid()).ToList())
            //{
            //    i++;
            //    ReadDirectory(dir, author);
            //    Console.WriteLine($"{i}/{n} {dir.Substring(dir.Length - 50, 50)}");
            //    if (i % 100 == 0) Context.SaveChanges();
            //    //if (i > 200 && all!=null) break;
            //}

            //Context.SaveChanges();
            //return StatusCode(200, dirs);
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

        private IList<string> ScanDirs(string parentDir)
        {
            var dirs = Directory.EnumerateDirectories(parentDir);
            var list = new List<string>();
            foreach (var dir in dirs) list.AddRange(ScanDirs(dir));
            if (list.Count == 0) list.Add(parentDir);
            return list;
        }


        private string convertImages(string inputText, string dir)
        {
            inputText = Regex.Replace(inputText, @"(\[zadania.info\]\(.*\))$", "$1.");

            var imageMatches = Regex.Matches(inputText, @"\!\[\]\((\w+\.\w+)\)", RegexOptions.Multiline);

            foreach (Match match in imageMatches)
            {
                var originalPart = match.Groups[0].Value;
                var filePath = match.Groups[1].Value;
                var fileBytes = System.IO.File.ReadAllBytes(Path.Join(dir, filePath));
                var base64 = Convert.ToBase64String(fileBytes);
                var replacementString = @$"<img src=""data:image/gif;base64, {base64}""/>";
                inputText = inputText.Replace(originalPart, replacementString);
            }

            var linksMatches = Regex.Matches(inputText, @"\[(.+?)\]\((.+?)\)");
            foreach (Match match in linksMatches)
            {
                var text = match.Groups[1].Value;
                var link = match.Groups[2].Value;
                inputText = inputText.Replace(match.Groups[0].Value, $@"<a href=""{link}"">{text}</a>");
            }

            return inputText;
        }

        private string TextReplace(string text, string dir)
        {
            text = convertImages(text, dir);
            var lines = text.Split('\n').Select(line => line.Trim()).Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => $"<p>{line}</p>");
            return string.Join("\n", lines);
        }

        private void ReadDirectory(string dir, Author author)
        {
            var exerciseText = System.IO.File.ReadAllText(Path.Join(dir, "exercise.md"));
            var solutionText = System.IO.File.ReadAllText(Path.Join(dir, "solution.md"));
            var exercise = TextReplace(exerciseText, dir);
            var solution = TextReplace(solutionText, dir);

            var title = exerciseText.Split("\n").First();

            if (title.Length > 100) title = title.Substring(0, 100);

            var titleList = title.Split(' ').Where(word => word.Length > 0 && !Regex.IsMatch(word, @"[\!\(\)]"))
                .Take(5);
            title = string.Join(' ', titleList) + "...";

            if (exercise.Length > 64 * 1024) return;
            if (solution.Length > 64 * 1024) return;

            var category = dir.Split('\\').TakeLast(2).First();

            var problem = new Problem
            {
                Title = title,
                Content = exercise,
                Created = DateTime.Now,
                AuthorId = author.Id,
                Author = author,
                Source = "zadania.info",
                Tags = new[] {new Tag {Name = "zadania.info"}, new Tag {Name = "Matematyka"}, new Tag {Name = category}}
            };

            //problem.ProblemTags = ProblemsController.RefreshTags(problem, Context).ToArray();
            problem.Tags = null;

            //TagsController.RefreshTags(problem,context);

            var answer = new Answer
            {
                Content = solution,
                Author = author,
                AuthorId = author.Id,
                Created = DateTime.Now,
                Parent = problem,
                IsApproved = true
            };

            problem.Answers = new[] {answer};
            Context.Problems.Add(problem);
            //context.SaveChanges();
        }

#if DEBUG
        [Route("upload-exercises")]
        [HttpPost]
#endif
        public void LoadMd()
        {
            var author = Context.Authors.FirstOrDefault(a => a.Name == "Igor Nowicki");
            var problems = GetProblemsFromDirectory("../../WIT-Zajecia/semestr-2/OAK");
            foreach (var problem in problems) Context.AddProblem(problem, author);
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
            var matches = Regex.Matches(text, @"### (?:Zad.*)[\s\n]*([^#]+)### (?:Rozw.*)[\s\n]*([^#]+)",
                RegexOptions.Multiline);
            var elements = matches.Select(m => new Element(m, path)).Select(e => e.GetProblem());

            foreach (var element in elements) yield return element;
        }

        private string ToHtml(string value)
        {
            var output = "";
            foreach (var line in value.Split("\n")) output += $"<p>{line.Trim()}</p>\n";

            return output;
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
                if (!tagMatches.Success) return;
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
                ContentProblem = ContentProblem.Replace("\r", "");
                ContentSolution = ContentSolution.Replace("\r", "");

                ContentProblem = Regex.Replace(ContentProblem, @"\(\./images/\d+/(\d+.gif)\)", @"($1)");
                ContentSolution = Regex.Replace(ContentSolution, @"\(\./images/\d+/(\d+.gif)\)", @"($1)");
            }

            //public string Title { get; }
            public string ContentProblem { get; }
            public string ContentSolution { get; }

            public IEnumerable<Tag> Tags { get; }
            public ICollection<FileData> ImagesProblem { get; }
            public ICollection<FileData> ImagesSolution { get; }

            public Problem GetProblem()
            {
                return new Problem
                {
                    ContentRaw = ContentProblem,
                    Tags = Tags,
                    FileData = ImagesProblem,
                    Answers = new[]
                    {
                        new Answer
                        {
                            ContentRaw = ContentSolution,
                            FileData = ImagesSolution
                        }
                    }
                };
            }
        }
    }
}