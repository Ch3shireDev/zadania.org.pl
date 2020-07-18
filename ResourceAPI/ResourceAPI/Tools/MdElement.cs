using System.Collections.Generic;
using System.Text.RegularExpressions;
using ResourceAPI.Models.MultipleChoice;
using ResourceAPI.Models.Post;

namespace ResourceAPI.Tools
{
    public class MdElement
    {
        public MdElement(string[] lines, int level = 1)
        {
            var contentMode = false;
            var newElementMode = false;
            var subLines = new List<string>();
            foreach (var line in lines)
            {
                var m = Regex.Match(line, $@"^#{{{level}}} (.*)$");
                if (m.Success)
                {
                    Title = m.Groups[1].Value;
                    contentMode = true;
                    continue;
                }

                var newElement = Regex.IsMatch(line, $@"^#{{{level + 1}}} (.*)$");

                if (contentMode && !newElement && !newElementMode)
                {
                    Content += line + "\n";
                    continue;
                }

                newElementMode = true;
                contentMode = false;
                if (newElement)
                {
                    if (subLines.Count > 0) Children.Add(new MdElement(subLines.ToArray(), level + 1));

                    subLines = new List<string>();
                }

                subLines.Add(line);
            }

            if (subLines.Count > 0) Children.Add(new MdElement(subLines.ToArray(), level + 1));

            Content = Content.Trim();
        }

        public string Title { get; set; }
        public string Content { get; set; }
        public List<MdElement> Children { get; set; } = new List<MdElement>();

        public MultipleChoiceQuestion ToQuestion(Author author)
        {
            var solutionText = Children.Count > 0 ? Children[0].Content : null;
            var correctLetter = "";

            if (!string.IsNullOrWhiteSpace(solutionText))
            {
                var match = Regex.Match(solutionText, "Odpowiedź[:]? ([A-Z])", RegexOptions.Multiline);
                correctLetter = match.Groups[1].Value.ToLower();
            }

            var matchGroup = Regex.Matches(Content, @"^([a-zA-Z])[\.\)] (.*)$", RegexOptions.Multiline);
            var answers = new List<MultipleChoiceAnswer>();
            foreach (Match m in matchGroup)
            {
                var isCorrect = m.Groups[1].Value.ToLower() == correctLetter;
                var answer = new MultipleChoiceAnswer
                    {Content = m.Groups[2].Value, IsCorrect = isCorrect, Author = author};
                answers.Add(answer);
            }

            Content = Regex.Replace(Content, @"^([a-zA-Z])[\.\)] (.*)$", "", RegexOptions.Multiline);

            var solution = Children.Count > 0 ? Children[0].Content.Trim() : null;
            if (!string.IsNullOrWhiteSpace(solution))
                solution = Regex.Replace(solution, @"^Odpowiedź[:]? [A-Z]$", "", RegexOptions.Multiline);
            var question = new MultipleChoiceQuestion
                {Content = Content.Trim(), Solution = solution, Answers = answers, Author = author};
            return question;
        }
    }
}