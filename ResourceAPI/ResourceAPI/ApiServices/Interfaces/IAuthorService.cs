using Microsoft.AspNetCore.Http;
using ResourceAPI.Models.Post;

namespace ResourceAPI.ApiServices.Interfaces
{
    public interface IAuthorService
    {
        public Author GetAuthor(HttpContext context);
    }
}