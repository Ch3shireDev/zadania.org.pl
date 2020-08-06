//using System.Collections.Generic;
//using FileDataLibrary;

//namespace ProblemLibrary
//{
//    public class ProblemElement
//    {
//        //public ProblemElement(Match match, string path)
//        //{
//        //    ContentProblem = match.Groups[1].Value;
//        //    ContentSolution = match.Groups[2].Value;

//        //    var dir = Path.GetDirectoryName(path);

//        //    ImagesProblem = Regex.Matches(ContentProblem, @"!\[\w*\]\((.*?)\)", RegexOptions.Multiline)
//        //        .Select(m => new FileData(m.Groups[1].Value, dir)).ToList();

//        //    ImagesSolution = Regex.Matches(ContentSolution, @"!\[\w*\]\((.*?)\)", RegexOptions.Multiline)
//        //        .Select(m => new FileData(m.Groups[1].Value, dir)).ToList();

//        //    var tagMatches = Regex.Match(ContentProblem, @"Tagi: (.*)");

//        //    //if (tagMatches.Success)
//        //    //{
//        //    //    var tags = tagMatches.Groups[1].Value.Split(";").Select(tag => tag.Trim())
//        //    //        .Where(tag => !string.IsNullOrWhiteSpace(tag))
//        //    //        .Select(tag => new Tag {Name = tag.Replace("...", "")})
//        //    //        .Select(tag => new Tag {Name = tag.Name, Url = tag.GenerateUrl()});

//        //    //    var dict = new Dictionary<string, Tag>();
//        //    //    foreach (var tag in tags)
//        //    //    {
//        //    //        if (dict.ContainsKey(tag.Url)) continue;
//        //    //        dict.Add(tag.Url, tag);
//        //    //    }

//        //    //    //Tags = dict.Select(p => p.Value).ToList();

//        //    //    ContentProblem = ContentProblem.Replace(tagMatches.Value, "").Trim();
//        //    //}

//        //    ContentProblem = ContentProblem.Replace("\r", "");
//        //    ContentSolution = ContentSolution.Replace("\r", "");

//        //    ContentProblem = Regex.Replace(ContentProblem, @"\(\./images/\d+/(\d+.gif)\)", @"($1)");
//        //    ContentSolution = Regex.Replace(ContentSolution, @"\(\./images/\d+/(\d+.gif)\)", @"($1)");
//        //}

//        public string ContentProblem { get; }
//        public string ContentSolution { get; }

//        //public IEnumerable<Tag> Tags { get; }
//        public ICollection<FileData> ImagesProblem { get; }
//        public ICollection<FileData> ImagesSolution { get; }

//        public Problem GetProblem()
//        {
//            var problem = new Problem
//            {
//                Content = ContentProblem,
//                //Tags = Tags,
//                FileData = ImagesProblem
//            };

//            if (!string.IsNullOrWhiteSpace(ContentSolution))
//                problem.Answers = new[]
//                {
//                    new Answer
//                    {
//                        Content = ContentSolution,
//                        FileData = ImagesSolution,
//                        IsApproved = true
//                    }
//                };


//            //if (string.IsNullOrWhiteSpace(problem.Name) && problem.Tags != null && problem.Tags.Any())
//            //    problem.Name = problem.Tags.Last().Name;

//            return problem;
//        }
//    }
//}

