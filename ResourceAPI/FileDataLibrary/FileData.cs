namespace FileDataLibrary
{
    public class FileData
    {
        public int Id { get; set; }
        public string OriginalFileName { get; set; }
        public string FileDir { get; set; }
        public string FileName { get; set; }
        public int ProblemId { get; set; }
        public int ExerciseId { get; set; }
        public int QuizTestId { get; set; }
        public int QuizQuestionId { get; set; }
        public int QuizAnswerId { get; set; }
        public int ProblemAnswerId { get; set; }
    }
}