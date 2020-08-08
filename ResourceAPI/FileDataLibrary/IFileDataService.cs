using System.Collections.Generic;

namespace FileDataLibrary
{
    public interface IFileDataService
    {
        public FileData Create(FileDataView fileData, int problemId = 0);
        public FileDataView Get(int id);
        public void Delete(int id);
        string GetAbsolutePath(FileData data);
        IEnumerable<FileDataView> GetFilesForProblem(int problemId);
    }
}