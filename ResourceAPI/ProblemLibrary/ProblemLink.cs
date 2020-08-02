namespace ProblemLibrary
{
    public class ProblemLink
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url => $"/api/v1/problems/{Id}";
    }
}