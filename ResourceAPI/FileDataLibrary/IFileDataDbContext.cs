using Microsoft.EntityFrameworkCore;

namespace FileDataLibrary
{
    public interface IFileDataDbContext
    {
        DbSet<FileData> FileData { get; set; }
        int SaveChanges(bool acceptAllChangesOnSuccess = true);
    }
}