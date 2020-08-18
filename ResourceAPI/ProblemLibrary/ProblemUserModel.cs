using System.Collections.Generic;
using CommonLibrary;
using FileDataLibrary;

namespace ProblemLibrary
{
    public class ProblemUserModel
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }

        public IEnumerable<FileDataView> Files { get; set; } = new List<FileDataView>();

        public Problem ToModel()
        {
            return new Problem
            {
                Name = Name,
                Content = Content,
                CategoryId = CategoryId,
                Files = Files
            };
        }

        public void Render()
        {
            Content = Tools.Render(Content, Files);
            Files = null;
        }
    }
}