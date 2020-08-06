using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace FileDataLibrary
{
    public class FileData
    {
        public FileData()
        {
        }

        public FileData(string filePath, string dir)
        {
            FileName = Path.GetFileName(filePath);
            FileDir = Path.GetDirectoryName(filePath);
            FileBytes = File.ReadAllBytes(Path.Combine(dir, filePath));
        }

        public int Id { get; set; }
        public string FileName { get; set; }

        public string FileDir { get; set; }

        [NotMapped] public byte[] FileBytes { get; set; }


        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            throw new NotImplementedException();
            //FileBytes = File.ReadAllBytes(Path.Combine(SqlContext.FileDirectory, FileDir, FileName));
        }
    }
}