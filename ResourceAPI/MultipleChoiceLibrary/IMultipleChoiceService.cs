using System.Collections.Generic;

namespace MultipleChoiceLibrary
{
    public interface IMultipleChoiceService
    {
        public MultipleChoiceTest GetTest(int testId, bool includeQuestions = true, bool includeAnswers = true);
        public MultipleChoiceQuestion GetQuestion(int questionId, bool includeAnswers = true);
        public MultipleChoiceAnswer GetAnswer(int answerId);
        public int CreateTest(int categoryId, MultipleChoiceTest element, int authorId = 1);
        public int CreateQuestion(int testId, MultipleChoiceQuestion question, int authorId = 1);
        public int CreateAnswer(int questionId, MultipleChoiceAnswer answer, int authorId = 1);
        int Create(MultipleChoiceTest multipleChoiceTest, int authorId = 1);
        bool EditTest(int testId, MultipleChoiceTest multipleChoiceTest);
        bool DeleteAnswer(int answerId);
        bool DeleteQuestion(int questionId);
        bool DeleteTest(int testId);
        bool EditAnswer(int answerId, MultipleChoiceAnswer answer);
        bool EditQuestion(int questionId, MultipleChoiceQuestion question);
        IEnumerable<MultipleChoiceTest> Browse();
    }
}