using System.Collections.Generic;
using ResourceAPI.Models.Post;
using ResourceAPI.Models.Problem;

namespace ResourceAPI.ApiServices.Interfaces
{
    public interface IProblemService
    {
        public Problem ProblemById(int id);
        Answer GetAnswerById(int problemId, int answerId);
        public Problem ProblemWithAnswersById(int id);
        int Create(int categoryId, Problem problem, Author author, bool withAnswers = false);

        IEnumerable<Problem> BrowseProblems(string tags, string query, bool newest, bool highest, int page,
            out int totalPages);
    }
}