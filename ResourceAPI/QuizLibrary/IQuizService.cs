using System.Collections.Generic;

namespace QuizLibrary
{
    public interface IQuizService
    {
        public Quiz GetTest(int testId, bool includeQuestions = true, bool includeAnswers = true);
        public QuizQuestion GetQuestion(int questionId, bool includeAnswers = true);
        public QuizAnswer GetAnswer(int answerId);
        public int CreateTest(int categoryId, Quiz element, int authorId = 1);
        public int CreateQuestion(int testId, QuizQuestion question, int authorId = 1);
        public int CreateAnswer(int questionId, QuizAnswer answer, int authorId = 1);
        int Create(Quiz quiz, int authorId = 1);
        bool EditTest(int testId, Quiz quiz);
        bool DeleteAnswer(int answerId);
        bool DeleteQuestion(int questionId);
        bool DeleteTest(int testId);
        bool EditAnswer(int answerId, QuizAnswer answer);
        bool EditQuestion(int questionId, QuizQuestion question);
        IEnumerable<Quiz> Browse();
    }
}