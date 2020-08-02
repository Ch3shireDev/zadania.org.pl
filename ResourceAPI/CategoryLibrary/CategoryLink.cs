namespace CategoryLibrary
{
    public class CategoryLink
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url => $"/api/v1/categories/{Id}";
    }
}