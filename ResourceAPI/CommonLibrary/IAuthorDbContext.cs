using Microsoft.EntityFrameworkCore;

namespace CommonLibrary
{
    public interface IAuthorDbContext
    {
        DbSet<Author> Authors { get; set; }
        int SaveChanges(bool acceptAllChangesOnSuccess = true);
    }
}