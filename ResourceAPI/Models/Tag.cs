namespace ResourceAPI.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PostCount { get; set; }
        public Problem Parent { get; set; }
        public int ParentId { get; set; }
    }
}