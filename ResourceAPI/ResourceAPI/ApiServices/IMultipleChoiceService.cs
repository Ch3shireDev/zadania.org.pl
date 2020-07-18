using ResourceAPI.Models.MultipleChoice;

namespace ResourceAPI.ApiServices
{
    public interface IMultipleChoiceService
    {
        public MultipleChoiceTest GetTestById(int testId, bool includeQuestions = false, bool includeAnswers = false);
        public MultipleChoiceQuestion GetQuestionById(int testId, int questionId, bool includeAnswers = false);

        public MultipleChoiceAnswer GetAnswerById(int testId, int questionId, int answerId);
    }
}