namespace MultipleChoiceLibrary
{
    public interface IMultipleChoiceService
    {
        public MultipleChoiceTest GetTestById(int testId, bool includeQuestions = false, bool includeAnswers = false);
        public MultipleChoiceQuestion GetQuestionById(int testId, int questionId, bool includeAnswers = false);
        public MultipleChoiceAnswer GetAnswerById(int testId, int questionId, int answerId);
        public int CreateTest(int categoryId, MultipleChoiceTest element, int authorId = 1);
        public int CreateQuestion(int testId, MultipleChoiceQuestion question, int authorId = 1);
        public int CreateAnswer(int questionId, MultipleChoiceAnswer answer, int authorId = 1);
        bool DeleteTest(int testId);
        int Create(MultipleChoiceTest multipleChoiceTest, int authorId = 1);
    }
}