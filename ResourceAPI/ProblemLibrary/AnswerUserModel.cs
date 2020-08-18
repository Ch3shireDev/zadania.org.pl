using FileDataLibrary;

namespace ProblemLibrary
{
    public class AnswerUserModel
    {
        public string Content { get; set; }
        public bool IsApproved { get; set; }
        public FileDataView[] Files { get; set; }

        public Answer ToModel()
        {
            return new Answer
            {
                Content = Content,
                Files = Files
            };
        }
    }
}