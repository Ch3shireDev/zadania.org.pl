using System.Linq;

namespace CommonLibrary
{
    public class MockAuthorService : IAuthorService
    {
        private readonly IAuthorDbContext _context;

        public MockAuthorService(IAuthorDbContext context)
        {
            _context = context;
            if (_context.Authors.Any()) return;
            _context.Authors.Add(new Author {Name = "Administrator"});
            _context.SaveChanges();
        }

        public int GetAuthor(string idClaim, UserData data)
        {
            return 1;
        }
    }
}