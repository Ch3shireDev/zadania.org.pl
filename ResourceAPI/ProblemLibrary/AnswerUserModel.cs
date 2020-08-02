namespace ProblemLibrary
{
    public class AnswerUserModel
    {
        public string Content { get; set; }
        public bool IsApproved { get; set; }

        public Answer ToModel()
        {
            return new Answer
            {
                Content = Content
            };
        }
    }
}