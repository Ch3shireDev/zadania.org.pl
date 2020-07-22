﻿using System.Collections.Generic;
using System.Linq;
using ResourceAPI.ApiServices.Interfaces;
using ResourceAPI.Models.Problem;

namespace ResourceAPI.ApiServices
{
    public class ProblemService : IProblemService
    {
        private readonly ICategoryService _categoryService;
        private readonly SqlContext _context;

        public ProblemService(SqlContext context, ICategoryService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
        }

        public Problem ProblemById(int id)
        {
            var problem = _context.Problems.Select(p => new Problem
                {
                    Id = p.Id,
                    Name = p.Name
                    //Content = p.Content,
                    //AuthorId = p.AuthorId,
                    //AuthorName = p.Author.Name,
                    //Created = p.Created,
                    //Edited = p.Edited,
                    //FileData = p.FileData,
                    //Tags = p.ProblemTags.Select(pt => new Tag {Name = pt.Tag.Name, Url = pt.Tag.Url}),
                    //Points = p.Points,
                    //UserUpvoted = p.ProblemVotes.Any(pv => pv.Vote == Vote.Upvote),
                    //UserDownvoted = p.ProblemVotes.Any(pv => pv.Vote == Vote.Downvote)
                })
                .FirstOrDefault(p => p.Id == id);
            if (problem == null) return null;

            problem.IsAnswered = _context.Answers.Where(a => a.ProblemId == problem.Id).Any(a => a.IsApproved);
            problem = problem.Render();
            problem.Content = null;
            return problem;
        }


        public Answer GetAnswerById(int problemId, int answerId)
        {
            var answer = _context.Answers.Select(a => new
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
                problem = _context.Problems.Select(p => new Problem
                    {
                        Id = p.Id,
                        Name = p.Name
                        //Content = p.Content,
                        //AuthorId = p.AuthorId,
                        //AuthorName = p.Author.Name,
                        //Answers = p.Answers.Select(a => new Answer {Id = a.Id, ProblemId = a.ProblemId}).ToList(),
                        //Created = p.Created,
                        //Edited = p.Edited,
                        //FileData = p.FileData,
                        //Tags = p.ProblemTags.Select(pt => new Tag {Name = pt.Tag.Name, Url = pt.Tag.Url}).ToArray(),
                        //Points = p.Points,
                        //UserUpvoted = p.ProblemVotes.Any(pv => pv.Vote == Vote.Upvote),
                        //UserDownvoted = p.ProblemVotes.Any(pv => pv.Vote == Vote.Downvote)
                    })
                    .FirstOrDefault(p => p.Id == id);
            if (problem == null) return null;

            problem.IsAnswered = _context.Answers.Where(a => a.ProblemId == problem.Id).Any(a => a.IsApproved);
            problem = problem.Render();


            return problem;
        }

        public int Create(int categoryId, Problem problem, int authorId = 1)
        {
            //if (!withAnswers) problem.Answers = null;
            //if (author == null) return 0;

            //var category = _categoryService.Get(categoryId);
            //if (category == null) return 0;

            //problem.Created = DateTime.Now;
            //problem.Author = author;

            //if (problem.FileData != null)
            //    foreach (var file in problem.FileData)
            //    {
            //        file.Save();
            //        var regex = @"!\[\]\(" + file.OldFileName + @"\)";
            //        problem.Content = Regex.Replace(problem.Content, regex, $"![]({file.FileName})");
            //    }

            //problem.ProblemTags ??= new List<ProblemTag>();
            //problem.Tags ??= new List<Tag>();

            //foreach (var tag in problem.Tags)
            //{
            //    tag.Url = tag.GenerateUrl();
            //    var existing = _context.Tags.Find(tag.Url) ?? tag;
            //    var problemTag = new ProblemTag {Tag = existing, TagUrl = tag.Url};
            //    problem.ProblemTags.Add(problemTag);
            //}

            //if (problem.Answers != null)
            //    foreach (var answer in problem.Answers)
            //    {
            //        if (answer.FileData == null) continue;
            //        foreach (var file in answer.FileData)
            //        {
            //            file.Save();
            //            var regex = @"!\[\]\(" + file.OldFileName + @"\)";
            //            answer.Content = Regex.Replace(answer.Content, regex, $"![]({file.FileName})");
            //        }

            //        answer.Author = author;
            //    }

            //problem.Tags = null;
            problem.CategoryId = categoryId;
            problem.AuthorId = authorId;
            _context.Problems.Add(problem);
            _context.SaveChanges();
            return problem.Id;
        }

        public IEnumerable<Problem> BrowseProblems(string tags, string query, bool newest, bool highest, int page,
            out int totalPages)
        {
            var tag = tags;
            var problemsQuery = _context.Problems.AsQueryable();

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


        public IEnumerable<ProblemTag> RefreshTags(Problem problem)
        {
            if (problem.Tags == null) yield break;
            foreach (var tag in problem.Tags)
            {
                tag.Url = tag.GenerateUrl();
                var existing = _context.Tags.Find(tag.Url) ?? tag;
                yield return new ProblemTag {Problem = problem, Tag = existing, TagUrl = tag.Url};
            }
        }
    }
}