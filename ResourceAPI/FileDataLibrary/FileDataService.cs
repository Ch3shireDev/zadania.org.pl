using System;
using System.IO;
using System.Linq;

namespace FileDataLibrary
{
    public class FileDataService : IFileDataService
    {
        private readonly IFileDataDbContext _context;

        public FileDataService(IFileDataDbContext context)
        {
            _context = context;
        }

        public static string FileDirectory { get; set; } = "../../images";

        public FileData Create(FileDataView fileData)
        {
            var path = CreateFile(fileData.FileBytes);

            var element = new FileData
            {
                FileName = fileData.FileName,
                SerializedFileName = Path.GetFileName(path),
                FileDir = Path.GetDirectoryName(path)
            };

            _context.FileData.Add(element);
            _context.SaveChanges();

            return element;
        }

        public FileDataView Get(int id)
        {
            var element = _context.FileData.FirstOrDefault(f => f.Id == id);
            if (element == null) return null;
            return new FileDataView
            {
                FileBytes = GetFile(element),
                FileName = element.FileName
            };
        }

        public void Remove(int id)
        {
            var element = _context.FileData.FirstOrDefault(f => f.Id == id);
            if (element == null) return;
            DeleteFile(element);
            _context.FileData.Remove(element);
            _context.SaveChanges();
        }

        private byte[] GetFile(FileData fileData)
        {
            return File.ReadAllBytes(Path.Combine(fileData.FileDir, fileData.SerializedFileName));
        }

        private string CreateFile(byte[] fileBytes)
        {
            if (!Directory.Exists(FileDirectory)) Directory.CreateDirectory(FileDirectory);
            var subdir = Directory.GetDirectories(FileDirectory).LastOrDefault()?.Replace("/", "\\").Split('\\').Last();
            if (subdir == null)
            {
                subdir = "000";
                Directory.CreateDirectory(Path.Combine(FileDirectory, subdir));
            }

            var filesCount = Directory.GetFiles(Path.Combine(FileDirectory, subdir)).Length;

            if (filesCount > 1000)
            {
                subdir = $"{Convert.ToInt32(subdir) + 1:D3}";
                filesCount = 0;
            }

            var ddir = Path.Combine(FileDirectory, subdir);
            if (!Directory.Exists(ddir)) Directory.CreateDirectory(ddir);
            var fname = $"{filesCount:D4}";
            var fpath = Path.Combine(ddir, fname);
            File.WriteAllBytes(fpath, fileBytes);
            return fpath;
        }

        private void DeleteFile(FileData element)
        {
            var fpath = Path.Combine(FileDirectory, element.FileDir, element.FileName);
            if (!File.Exists(fpath)) return;
            File.Delete(fpath);
        }
    }
}