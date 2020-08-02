namespace QuizLibrary
{
    public class QuizUserModel
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }

        public Quiz ToModel()
        {
            return new Quiz
            {
                Name = Name,
                Content = Content,
                CategoryId = CategoryId
            };
        }
    }
}