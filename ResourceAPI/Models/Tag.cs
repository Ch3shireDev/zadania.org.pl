using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using MySql.Data.EntityFrameworkCore.DataAnnotations;

namespace ResourceAPI.Models
{
    public class Tag
    {
        //public int Id { get; set; }
        [Key] public string Url { get; set; }

        [MySqlCharset("utf8")] public string Name { get; set; }

        public int PostCount { get; set; }
        public ICollection<ProblemTag> ProblemTags { get; set; }

        public string GenerateUrl()
        {
            var url = Name
                    .ToLowerInvariant()
                    .Replace("ą", "a")
                    .Replace("ę", "e")
                    .Replace("ł", "l")
                    .Replace("ó", "o")
                    .Replace("ź", "z")
                    .Replace("ż", "z")
                    .Replace("ś", "s")
                ;
            return Regex.Replace(url, @"[^a-z\d]+", "_");
        }

        public Tag Serializable(int depth = 0)
        {
            ProblemTags = null;
            return this;
        }
    }
}