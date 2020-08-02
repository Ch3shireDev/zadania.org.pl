namespace CategoryLibrary
{
    public class CategoryView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ParentUrl { get; set; }
        public int CategoriesCount { get; set; }
        public CategoryLink[] Categories { get; set; }
        public int ProblemsCount { get; set; }
        public string ProblemsUrl => $"/api/v1/categories/{Id}/problems";
        public int ExercisesCount { get; set; }
        public string ExercisesUrl => $"/api/v1/categories/{Id}/exercises";
        public int QuizzesCount { get; set; }
        public string QuizzesUrl => $"/api/v1/categories/{Id}/quizzes";
    }
}