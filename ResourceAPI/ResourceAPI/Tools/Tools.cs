using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ResourceAPI.Models.Post;

namespace ResourceAPI.Tools
{
    public static class Tools
    {
        public static string Render(string contentRaw, ICollection<FileData> fileData)
        {
            if (contentRaw == null) return null;
            var html = contentRaw;
            html = Regex.Replace(html, @"!\[\]\(([^)]+)\)", "<img src='$1'/>", RegexOptions.Multiline);
            html = Regex.Replace(html, @"\[(.+)\]\((.+?)\)", "<a href=\"$2\">$1</a>", RegexOptions.Multiline);
            var lines = html.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => $"<p>{x}</p>");
            var content = string.Join('\n', lines);
            if (fileData == null) return content;
            foreach (var file in fileData)
            {
                if (file.FileBytes == null) file.Load();
                if (file.FileBytes == null) continue;
                var data = $"data:image/gif;base64,{Convert.ToBase64String(file.FileBytes)}";
                content = content.Replace(file.FileName, data);
            }

            return content;
        }
    }
}