using System.Collections.Generic;
using CommonLibrary;

namespace ProblemLibrary
{
    public interface IProblemService
    {
        public Problem ProblemById(int id);
        Answer GetAnswerById(int problemId, int answerId);
        public Problem Get(int problemId);
        int Create(Problem problem, int authorId = 1);

        IEnumerable<Problem> BrowseProblems(int page,
            out int totalPages, string tags = null, string query = null, bool newest = true, bool highest = false);

        bool Edit(int problemId, Problem problem, int authorId = 1);
        bool Delete(int problemId);
        int CreateAnswer(int problemId, Answer answer, int authorId = 1);
        Answer GetAnswer(int problemId, int answerId);
        bool EditAnswer(int problemId, int answerId, Answer answer, int authorId = 1);
        bool DeleteAnswer(int problemId, int answerId);
        void SetAnswerApproval(int problemId, int answerId1, bool isApproved = true);
        void VoteProblem(int id, Vote vote, int authorId = 1);

        //public ProblemViewModel GetProblemView(int id);
    }
}