using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using FileDataLibrary;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace CommonLibrary
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

        public static int GetAuthorId(IAuthorService authorService, HttpContext httpContext)
        {
            if (authorService == null) return 1;
            if (httpContext == null) return 0;
            var profileData = httpContext.Request.Headers["profile"];
            if (profileData.Count == 0) return authorService.GetAuthor(null, null);
            var profile = JsonConvert.DeserializeObject<UserData>(profileData);
            var httpContextUser = httpContext.User;
            var claim = httpContextUser.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var idClaim = claim?.Value;
            return idClaim == null ? 0 : authorService.GetAuthor(idClaim, profile);
        }
    }
}