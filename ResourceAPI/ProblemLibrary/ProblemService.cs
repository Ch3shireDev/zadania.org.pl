using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CommonLibrary;

namespace ProblemLibrary
{
    public class ProblemService : IProblemService
    {
        private readonly IProblemDbContext _context;
        private readonly IMapper _mapper;

        public ProblemService(IProblemDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Problem ProblemById(int id)
        {
            var problem = _context.Problems.Select(p => new Problem
                {
                    Id = p.Id,
                    Name = p.Name,
                    Content = p.Content
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

            problem.IsSolved = _context.Answers.Where(a => a.ProblemId == problem.Id).Any(a => a.IsApproved);
            //problem = problem.Render();
            //problem.Content = null;

            return problem;
        }

        public ProblemViewModel GetProblemView(int id)
        {
            var problem = ProblemById(id);
            var problemView = _mapper.Map<ProblemViewModel>(problem);
            return problemView;
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
                        //Answers = p.Answers.Select(a => new Answer {Id = a.Id, ProblemId = a.ProblemId}).ToList(),
                        //Created = p.Created,
                        //Edited = p.Edited,
                        //FileData = p.FileData,
                        //Tags = p.ProblemTags.Select(pt => new Tag {Name = pt.Tag.Name, Url = pt.Tag.Url}).ToArray(),
                        //Points = p.Points,
                        //UserUpvoted = p.ProblemVotes.Any(pv => pv.Vote == Vote.Upvote),
                        //UserDownvoted = p.ProblemVotes.Any(pv => pv.Vote == Vote.Downvote)
                    })
                    .FirstOrDefault(p => p.Id == problemId);
            if (problem == null) return null;

            problem.IsSolved = _context.Answers.Where(a => a.ProblemId == problem.Id).Any(a => a.IsApproved);
            problem = problem.Render();
            problem.Answers = problem.Answers.Select(a => a.Render()).ToList();
            return problem;
        }

        public int Create(Problem problem, int authorId = 1)
        {
            if (problem.CategoryId == 0) problem.CategoryId = 1;
            //if (!withAnswers) problem.Answers = null;
            //if (author == null) return 0;

            //var category = _categoryService.GetProblems(categoryId);
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
            //problem.CategoryId = categoryId;
            problem.AuthorId = authorId;
            _context.Problems.Add(problem);
            _context.SaveChanges();
            return problem.Id;
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
                    .Select(p => ProblemById(p.Id)).ToArray()
                ;

            totalPages = num % 10 == 0 ? num / 10 : num / 10 + 1;

            return problems;
        }

        public bool Edit(int problemId, Problem problem)
        {
            var element = _context.Problems.FirstOrDefault(p => p.Id == problemId);
            if (element == null) return false;
            element.Name = problem.Name;
            element.Content = problem.Content;
            _context.Problems.Update(element);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(int problemId)
        {
            var problem = _context.Problems.FirstOrDefault(p => p.Id == problemId);
            if (problem == null) return false;
            _context.Problems.Remove(problem);
            _context.SaveChanges();
            return true;
        }

        public int CreateAnswer(int problemId, Answer answer, int authorId = 1)
        {
            var problem = _context.Problems.FirstOrDefault(p => p.Id == problemId);
            if (problem == null) return 0;

            //var author = _context.Authors.FirstOrDefault(a => a.Id == authorId);
            //if (author == null) return 0;

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
            return answer?.Render();
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

        public bool DeleteAnswer(int problemId, int answerId)
        {
            var element = _context.Answers.FirstOrDefault(a => a.ProblemId == problemId && a.Id == answerId);
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

        public void VoteProblem(int problemId, Vote vote, int authorId)
        {
            //var problemVote =
            //    _context.ProblemVotes.FirstOrDefault(pv => pv.AuthorId == authorId && pv.ProblemId == problemId);
            //if (problemVote == null)
            //{
            //    problemVote = new ProblemVote { AuthorId = authorId, ProblemId = problemId };
            //    _context.ProblemVotes.Add(problemVote);
            //}
            //else
            //{
            //    problemVote.Vote = problemVote.Vote == vote ? Vote.None : vote;
            //    _context.ProblemVotes.Update(problemVote);
            //}

            //_context.SaveChanges();

            //var problem = Get(problemId);

            //problem.Points = _context.ProblemVotes.Where(pv => pv.ProblemId == problem.Id)
            //    .Select(pv => pv.Vote == Vote.Upvote ? 1 : pv.Vote == Vote.Downvote ? -1 : 0)
            //    .Sum();

            //_context.Problems.Update(problem);

            //_context.SaveChanges();
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