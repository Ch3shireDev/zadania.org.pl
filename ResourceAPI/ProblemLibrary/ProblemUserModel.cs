﻿using System.Collections.Generic;
using FileDataLibrary;

namespace ProblemLibrary
{
    public class ProblemUserModel
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }

        public IEnumerable<FileDataView> Files { get; set; }

        public Problem ToModel()
        {
            return new Problem
            {
                Name = Name,
                Content = Content,
                CategoryId = CategoryId
            };
        }
    }
}