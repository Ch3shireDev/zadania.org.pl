namespace FileDataLibrary
{
    public interface IFileDataService
    {
        public FileData Create(FileDataView fileData);
        public FileDataView Get(int id);
        public void Remove(int id);
    }
}