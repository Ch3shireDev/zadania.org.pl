namespace ExerciseLibrary
{
    public class ExerciseLink
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url => $"/api/v1/exercises/{Id}";
    }
}