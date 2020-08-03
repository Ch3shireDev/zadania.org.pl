using System.Collections.Generic;
using System.Linq;

namespace ProblemLibrary
{
    public class ProblemService : IProblemService
    {
        private readonly IProblemDbContext _context;

        public ProblemService(IProblemDbContext context)
        {
            _context = context;
        }

        public Problem Get(int problemId)
        {
            var
                problem = _context.Problems.Select(p => new Problem
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Content = p.Content,
                        AuthorId = p.AuthorId,
                        IsSolved = p.IsSolved,
                        CategoryId = p.CategoryId,
                        AuthorName = p.Author.Name,
                        Answers = p.Answers.Select(a => new Answer
                                {Id = a.Id, Content = a.Content, AuthorName = a.AuthorName, IsApproved = a.IsApproved})
                            .ToList()
                    })
                    .FirstOrDefault(p => p.Id == problemId);
            if (problem == null) return null;

            problem.IsSolved = _context.Answers.Where(a => a.ProblemId == problem.Id).Any(a => a.IsApproved);
            //problem = problem.Render();
            problem.Answers = problem.Answers.Select(a => a.Render()).ToList();
            return problem;
        }

        public Problem Create(Problem problem, int authorId = 1)
        {
            if (problem.CategoryId == 0) problem.CategoryId = 1;
            //if (!withAnswers) problem.Answers = null;
            //if (author == null) return 0;

            //var category = _categoryService.GetProblems(categoryId);
            //if (category == null) return 0;

            problem.AuthorId = authorId;
            _context.Problems.Add(problem);

            _context.SaveChanges();
            return problem;
        }

        public IEnumerable<Problem> BrowseProblems(int page,
            out int totalPages, string tags, string query, bool newest, bool highest)
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
                    .Select(p => Get(p.Id)).ToArray()
                ;

            totalPages = num % 10 == 0 ? num / 10 : num / 10 + 1;

            return problems;
        }

        public bool Edit(Problem problem, int problemId, int authorId)
        {
            var element = _context.Problems.FirstOrDefault(p => p.Id == problemId);
            if (element == null) return false;
            element.Name = problem.Name;
            element.Content = problem.Content;
            _context.Problems.Update(element);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(int problemId, int authorId = 1)
        {
            var problem = _context.Problems.FirstOrDefault(p => p.Id == problemId && p.AuthorId == authorId);
            if (problem == null) return false;
            _context.Problems.Remove(problem);
            _context.SaveChanges();
            return true;
        }

        public int CreateAnswer(int problemId, Answer answer, int authorId = 1)
        {
            var problem = _context.Problems.FirstOrDefault(p => p.Id == problemId);
            if (problem == null) return 0;

            var element = new Answer
            {
                Content = answer.Content,
                ProblemId = problemId,
                AuthorId = authorId
            };

            _context.Answers.Add(element);
            _context.SaveChanges();

            return element.Id;
        }

        public Answer GetAnswer(int problemId, int answerId)
        {
            var answer = _context.Answers.FirstOrDefault(a => a.ProblemId == problemId && a.Id == answerId);
            return answer;
        }

        public bool EditAnswer(int problemId, int answerId, Answer answer, int authorId = 1)
        {
            var element = _context.Answers.FirstOrDefault(a => a.ProblemId == problemId && a.Id == answerId);
            if (element == null) return false;
            element.Content = answer.Content;
            _context.Answers.Update(element);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteAnswer(int problemId, int answerId, int authorId)
        {
            var element = _context.Answers.FirstOrDefault(a =>
                a.ProblemId == problemId && a.Id == answerId && a.AuthorId == authorId);
            if (element == null) return false;
            _context.Answers.Remove(element);
            _context.SaveChanges();
            return true;
        }

        public void SetAnswerApproval(int problemId, int answerId, bool isApproved = true)
        {
            var answer = _context.Answers.FirstOrDefault(a => a.ProblemId == problemId && a.Id == answerId);
            if (answer == null) return;
            if (answer.IsApproved == isApproved) return;

            var problem = _context.Problems.FirstOrDefault(p => p.Id == problemId);
            if (problem == null) return;

            if (isApproved)
            {
                var answers = _context.Answers.Where(a => a.ProblemId == problemId).ToList();
                foreach (var element in answers) element.IsApproved = element.Id == answerId;
                _context.Answers.UpdateRange(answers);
                if (!problem.IsSolved)
                {
                    problem.IsSolved = true;
                    _context.Problems.Update(problem);
                }
            }
            else
            {
                answer.IsApproved = false;
                _context.Answers.Update(answer);
                if (problem.IsSolved)
                {
                    problem.IsSolved = false;
                    _context.Problems.Update(problem);
                }
            }

            _context.SaveChanges();
        }
    }
}