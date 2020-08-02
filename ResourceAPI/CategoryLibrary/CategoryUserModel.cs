namespace CategoryLibrary
{
    public class CategoryUserModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Category ToModel()
        {
            return new Category
            {
                Name = Name,
                Description = Description
            };
        }
    }
}