using System.Collections.Generic;
using System.Linq;
using ResourceAPI.Models;

namespace ResourceAPI.Services
{
    public interface IProblemService
    {
        public Problem GetById(int id);
        Answer GetAnswerById(int problemId, int answerId);
    }

    public class ProblemService : IProblemService
    {
        public ProblemService(SqlContext context)
        {
            Context = context;
        }

        public SqlContext Context { get; set; }

        private static Dictionary<int, Problem> ProblemsDictionary { get; } = new Dictionary<int, Problem>();
        private static Dictionary<int, Answer> AnswersDictionary { get; } = new Dictionary<int, Answer>();

        public Problem GetById(int id)
        {
            if (ProblemsDictionary.ContainsKey(id)) return ProblemsDictionary[id];

            var problem = Context.Problems.Select(p => new Problem
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    AuthorId = p.AuthorId,
                    AuthorName = p.Author.Name,
                    AnswerLinks = p.Answers.Select(a => $"/api/v1/problems/{id}/answers/{a.Id}"),
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
            ProblemsDictionary.Add(id, problem);
            return problem;
        }

        public Answer GetAnswerById(int problemId, int answerId)
        {
            if (AnswersDictionary.ContainsKey(answerId))
            {
                var answerDict = AnswersDictionary[answerId];
                if (answerDict.ProblemId != problemId) return null;
                return answerDict;
            }

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
            AnswersDictionary.Add(answerId, answer);
            return answer;
        }
    }
}