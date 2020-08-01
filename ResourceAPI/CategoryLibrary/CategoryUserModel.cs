namespace CategoryLibrary
{
    public class CategoryUserModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Category AsModel()
        {
            return new Category
            {
                Name = Name,
                Description = Description
            };
        }
    }
}