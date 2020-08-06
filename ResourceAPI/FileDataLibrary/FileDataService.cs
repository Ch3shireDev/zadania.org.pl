using System;
using System.IO;
using System.Linq;

namespace FileDataLibrary
{
    public class FileDataService : IFileDataService
    {
        public static string FileDirectory { get; set; } = "../../images";

        public void AddFile(FileData fileData)
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

            var fileName = fileData.FileName;
            var fileBytes = fileData.FileBytes;

            var ddir = Path.Combine(FileDirectory, subdir);
            if (!Directory.Exists(ddir)) Directory.CreateDirectory(ddir);
            var ext = Path.GetExtension(fileName);
            var fname = $"{filesCount:D4}{ext}";
            File.WriteAllBytes(Path.Combine(ddir, fname), fileBytes);
        }
    }
}