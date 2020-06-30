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
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        public AdminController(ILogger<ProblemsController> logger, SqlContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        private ILogger<ProblemsController> logger { get; }
        private SqlContext context { get; }

        private IList<string> ScanDirs(string parentDir)
        {
            var dirs = Directory.EnumerateDirectories(parentDir);
            var list = new List<string>();
            foreach (var dir in dirs) list.AddRange(ScanDirs(dir));
            if (list.Count == 0) list.Add(parentDir);
            return list;
        }

#if DEBUG
        [HttpPost]
        [Route("upload")]
#endif
        public ActionResult PostFiles()
        {
            var curr = Directory.GetCurrentDirectory();
            var dirs = ScanDirs(Path.Join(curr, "../zadania.info/zadania-info"));

            var author = context.Authors.FirstOrDefault(auth => auth.Name == "zadania.info");
            if (author == null)
            {
                author = new Author {Name = "zadania.info"};
                context.Authors.Add(author);
                context.SaveChanges();
            }

            var i = 0;
            var n = dirs.Count;
            foreach (var dir in dirs.OrderBy(a => Guid.NewGuid()).ToList())
            {
                i++;
                ReadDirectory(dir, author);
                Console.WriteLine($"{i}/{n} {dir.Substring(dir.Length - 50, 50)}");
                if (i % 100 == 0) context.SaveChanges();
                if (i > 200) break;
            }

            context.SaveChanges();
            return StatusCode(200, dirs);
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

            var title = "";
            if (exerciseText.Length > 100) title = exerciseText.Substring(0, 100);
            else title = exerciseText;
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

            problem.ProblemTags = ProblemsController.RefreshTags(problem, context).ToArray();
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
            context.Problems.Add(problem);
            //context.SaveChanges();
        }
    }
}