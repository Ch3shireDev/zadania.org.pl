using System.Collections.Generic;

namespace ProblemLibrary
{
    public interface IProblemService
    {
        //public Problem GetProblem(int id);
        //Answer GetAnswerById(int problemId, int answerId);
        public Problem Get(int problemId);
        Problem Create(Problem problem, int categoryId = 1, int authorId = 1);

        IEnumerable<Problem> BrowseProblems(int page,
            out int totalPages, string tags = null, string query = null, bool newest = true);

        bool Edit(Problem problem, int problemId, int authorId = 1);
        bool Delete(int problemId, int authorId = 1);
        int CreateAnswer(int problemId, Answer answer, int authorId = 1);
        Answer GetAnswer(int problemId, int answerId);
        bool EditAnswer(int problemId, int answerId, Answer answer, int authorId = 1);
        bool DeleteAnswer(int problemId, int answerId, int authorId = 1);

        void SetAnswerApproval(int problemId, int answerId1, bool isApproved = true);
        //void VoteProblem(int id, Vote vote, int authorId = 1);

        //public ProblemView GetProblemView(int id);
    }
}