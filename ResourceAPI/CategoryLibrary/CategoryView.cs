using ProblemLibrary;

namespace CategoryLibrary
{
    public class CategoryView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ParentUrl { get; set; }
        public CategoryLink[] Categories { get; set; }
        public ProblemLink[] Problems { get; set; }
    }
}