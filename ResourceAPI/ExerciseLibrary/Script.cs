namespace ExerciseLibrary
{
    public class Script
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public VariableType Type { get; set; }
        public int? IntMin { get; set; }
        public int? IntMax { get; set; }
        public float? FloatMin { get; set; }
        public float? FloatMax { get; set; }
        public int ExerciseId { get; set; }
    }
}