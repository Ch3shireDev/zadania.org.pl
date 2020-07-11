using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using ResourceAPI.Models;

namespace ResourceAPI
{
    public class SqlContext : DbContext
    {
        public SqlContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Answer> Answers { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProblemTag> ProblemTags { get; set; }

        public DbSet<ProblemVote> ProblemVotes { get; set; }
        public DbSet<AnswerVote> AnswerVotes { get; set; }

        public static string ConnectionString { get; set; }
        public static string FileDirectory { get; set; } = "../../images";
        public DbSet<MultipleChoiceTest> MultipleChoiceTests { get; set; }
        public DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }
        public DbSet<MultipleChoiceAnswer> MultipleChoiceAnswers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProblemTag>()
                .HasKey(pc => new {pc.TagUrl, pc.ProblemId});

            modelBuilder.Entity<ProblemTag>()
                .HasOne(pc => pc.Problem)
                .WithMany(p => p.ProblemTags);

            modelBuilder.Entity<ProblemTag>()
                .HasOne(category => category.Tag)
                .WithMany(tag => tag.ProblemTags)
                .HasForeignKey(pc => pc.TagUrl);

            modelBuilder.Entity<ProblemTag>()
                .HasOne(pt => pt.Problem)
                .WithMany(p => p.ProblemTags)
                .HasForeignKey(pt => pt.ProblemId);

            modelBuilder.Entity<ProblemVote>().HasKey(pv => new {pv.ProblemId, pv.AuthorId});
            modelBuilder.Entity<ProblemVote>().HasOne(pv => pv.Problem).WithMany(p => p.ProblemVotes);
            modelBuilder.Entity<ProblemVote>().HasOne(pv => pv.Author).WithMany(a => a.ProblemVotes);

            modelBuilder.Entity<AnswerVote>().HasKey(pv => new {pv.AnswerId, pv.AuthorId});
            modelBuilder.Entity<AnswerVote>().HasOne(av => av.Answer).WithMany(a => a.AnswerVotes);
            modelBuilder.Entity<AnswerVote>().HasOne(av => av.Author).WithMany(a => a.AnswerVotes);
        }


        public static string Render(string contentRaw, ICollection<FileData> fileData)
        {
            if (contentRaw == null) return null;
            var html = contentRaw;
            html = Regex.Replace(html, @"!\[\]\(([^)]+)\)", "<img src='$1'/>", RegexOptions.Multiline);
            html = Regex.Replace(html, @"\[(.+)\]\((.+?)\)", "<a href=\"$2\">$1</a>", RegexOptions.Multiline);
            var lines = html.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => $"<p>{x}</p>");
            var content = string.Join('\n', lines);
            if (fileData == null) return content;
            foreach (var file in fileData)
            {
                if (file.FileBytes == null) file.Load();
                if (file.FileBytes == null) continue;
                var data = $"data:image/gif;base64,{Convert.ToBase64String(file.FileBytes)}";
                content = content.Replace(file.FileName, data);
            }

            return content;
        }

        public bool AddProblem(Problem problem, Author author = null, bool withAnswers = false)
        {
            if (!withAnswers) problem.Answers = null;
            if (author == null) return false;

            problem.Created = DateTime.Now;
            problem.Author = author;

            if (problem.FileData != null)
                foreach (var file in problem.FileData)
                {
                    file.Save();
                    var regex = @"!\[\]\(" + file.OldFileName + @"\)";
                    problem.Content = Regex.Replace(problem.Content, regex, $"![]({file.FileName})");
                }

            problem.ProblemTags ??= new List<ProblemTag>();
            problem.Tags ??= new List<Tag>();

            foreach (var tag in problem.Tags)
            {
                tag.Url = tag.GenerateUrl();
                var existing = Tags.Find(tag.Url) ?? tag;
                var problemTag = new ProblemTag {Tag = existing, TagUrl = tag.Url};
                problem.ProblemTags.Add(problemTag);
            }

            if (problem.Answers != null)
                foreach (var answer in problem.Answers)
                {
                    if (answer.FileData == null) continue;
                    foreach (var file in answer.FileData)
                    {
                        file.Save();
                        var regex = @"!\[\]\(" + file.OldFileName + @"\)";
                        answer.Content = Regex.Replace(answer.Content, regex, $"![]({file.FileName})");
                    }

                    answer.Author = author;
                }

            problem.Tags = null;
            Problems.Add(problem);
            return true;
        }

        public IEnumerable<ProblemTag> RefreshTags(Problem problem)
        {
            if (problem.Tags == null) yield break;
            foreach (var tag in problem.Tags)
            {
                tag.Url = tag.GenerateUrl();
                var existing = Tags.Find(tag.Url) ?? tag;
                yield return new ProblemTag {Problem = problem, Tag = existing, TagUrl = tag.Url};
            }
        }
    }
}