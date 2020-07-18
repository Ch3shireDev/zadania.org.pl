using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ResourceAPI.Enums;
using ResourceAPI.Models.Post;
using ResourceAPI.Models.Problem;

namespace ResourceAPI.ApiServices
{
    public class ProblemService : IProblemService
    {
        public ProblemService(SqlContext context)
        {
            Context = context;
        }

        public SqlContext Context { get; set; }


        public Problem ProblemById(int id)
        {
            var problem = Context.Problems.Select(p => new Problem
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    AuthorId = p.AuthorId,
                    AuthorName = p.Author.Name,
                    Created = p.Created,
                    Edited = p.Edited,
                    FileData = p.FileData,
                    Tags = p.ProblemTags.Select(pt => new Tag {Name = pt.Tag.Name, Url = pt.Tag.Url}).ToArray(),
                    Points = p.Points,
                    UserUpvoted = p.ProblemVotes.Any(pv => pv.Vote == Vote.Upvote),
                    UserDownvoted = p.ProblemVotes.Any(pv => pv.Vote == Vote.Downvote)
                })
                .FirstOrDefault(p => p.Id == id);
            if (problem == null) return null;

            problem.IsAnswered = Context.Answers.Where(a => a.ProblemId == problem.Id).Any(a => a.IsApproved);
            problem = problem.Render();
            problem.Content = null;
            return problem;
        }


        public Answer GetAnswerById(int problemId, int answerId)
        {
            var answer = Context.Answers.Select(a => new
                Answer
                {
                    Id = a.Id,
                    ProblemId = a.ProblemId,
                    IsApproved = a.IsApproved,
                    Content = a.Content,
                    Points = a.Points,
                    Edited = a.Edited,
                    Created = a.Created,
                    AuthorId = a.AuthorId,
                    AuthorName = a.Author.Name,
                    UserId = a.Author.UserId,
                    FileData = a.FileData
                }).FirstOrDefault(a => a.ProblemId == problemId && a.Id == answerId);
            if (answer == null) return null;
            answer = answer.Render();
            return answer;
        }

        public Problem ProblemWithAnswersById(int id)
        {
            var
                problem = Context.Problems.Select(p => new Problem
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Content = p.Content,
                        AuthorId = p.AuthorId,
                        AuthorName = p.Author.Name,
                        Answers = p.Answers.Select(a => new Answer {Id = a.Id, ProblemId = a.ProblemId}).ToList(),
                        Created = p.Created,
                        Edited = p.Edited,
                        FileData = p.FileData,
                        Tags = p.ProblemTags.Select(pt => new Tag {Name = pt.Tag.Name, Url = pt.Tag.Url}).ToArray(),
                        Points = p.Points,
                        UserUpvoted = p.ProblemVotes.Any(pv => pv.Vote == Vote.Upvote),
                        UserDownvoted = p.ProblemVotes.Any(pv => pv.Vote == Vote.Downvote)
                    })
                    .FirstOrDefault(p => p.Id == id);
            if (problem == null) return null;

            problem.IsAnswered = Context.Answers.Where(a => a.ProblemId == problem.Id).Any(a => a.IsApproved);
            problem = problem.Render();


            return problem;
        }

        public bool AddProblem(Problem problem, Author author, bool withAnswers = false)
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
                var existing = Context.Tags.Find(tag.Url) ?? tag;
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
            Context.Problems.Add(problem);
            return true;
        }

        public IEnumerable<Problem> BrowseProblems(string tags, string query, bool newest, bool highest, int page,
            out int totalPages)
        {
            var tag = tags;
            var problemsQuery = Context.Problems.AsQueryable();

            if (tags != null)
                problemsQuery = problemsQuery
                    .Where(p => p.ProblemTags.Select(pt => pt.Tag.Url).Any(t => t == tag));

            if (query != null)
                problemsQuery = problemsQuery.Where(p =>
                    p.Content.Contains(query) || p.ProblemTags.Any(pt => pt.Tag.Name.Contains(query)));


            var resultQuery = problemsQuery.AsQueryable();

            var linksQuery = resultQuery;

            if (newest)
                linksQuery = linksQuery
                    .OrderByDescending(p => p.Created)
                    .AsQueryable();

            if (highest) linksQuery = linksQuery.OrderByDescending(p => p.Points).AsQueryable();

            var num = resultQuery.Count();

            var lastRecordIndex = page * 10;
            var firstRecordIndex = lastRecordIndex - 10;

            var subQuery = linksQuery;

            if (firstRecordIndex < num) subQuery = subQuery.Skip(firstRecordIndex);
            if (lastRecordIndex < num) subQuery = subQuery.Take(10);

            var problems = subQuery.Select(p => new Problem {Id = p.Id}).ToArray()
                    .Select(p => ProblemById(p.Id)).ToArray()
                ;

            totalPages = num % 10 == 0 ? num / 10 : num / 10 + 1;

            return problems;
        }
    }
}