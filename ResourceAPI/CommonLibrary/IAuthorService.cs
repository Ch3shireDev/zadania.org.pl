namespace CommonLibrary
{
    public interface IAuthorService
    {
        //public Author GetAuthor(HttpContext context);

        public int GetAuthor(string idClaim, UserData data);
    }
}