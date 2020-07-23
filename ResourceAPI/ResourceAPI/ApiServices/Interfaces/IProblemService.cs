using System.Collections.Generic;
using ResourceAPI.Models.Problem;

namespace ResourceAPI.ApiServices.Interfaces
{
    public interface IProblemService
    {
        public Problem ProblemById(int id);
        Answer GetAnswerById(int problemId, int answerId);
        public Problem Get(int categoryId, int problemId);
        int Create(int categoryId, Problem problem, int authorId = 1);

        IEnumerable<Problem> BrowseProblems(int page,
            out int totalPages, string tags = null, string query = null, bool newest = true, bool highest = false);

        bool Edit(int categoryId, int problemId, Problem problem);
        bool Delete(int categoryId, int problemId);
        int CreateAnswer(int problemId, Answer answer, int authorId = 1);
        Answer GetAnswer(int problemId, int answerId);
        bool EditAnswer(int problemId, int answerId, Answer answer, int authorId = 1);
        bool DeleteAnswer(int problemId, int answerId);
    }
}