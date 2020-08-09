namespace QuizLibrary
{
    public class QuizLink
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url => $"/api/v1/quiz/{Id}";
    }
}