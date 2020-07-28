using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace CommonLibrary
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

        [NotMapped] public string OldFileName { get; set; }


        public void Save()
        {
            throw new NotImplementedException();
            //var dir = SqlContext.FileDirectory;
            //if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            //var subdir = Directory.GetDirectories(dir).LastOrDefault()?.Replace("/", "\\").Split('\\').Last();
            //if (subdir == null)
            //{
            //    subdir = "000";
            //    Directory.CreateDirectory(Path.Combine(dir, subdir));
            //}

            //var filesCount = Directory.GetFiles(Path.Combine(dir, subdir)).Length;

            //if (filesCount > 1000)
            //{
            //    subdir = $"{Convert.ToInt32(subdir) + 1:D3}";
            //    filesCount = 0;
            //}

            //var ddir = Path.Combine(dir, subdir);
            //if (!Directory.Exists(ddir)) Directory.CreateDirectory(ddir);
            //var ext = Path.GetExtension(FileName);
            //var fname = $"{filesCount:D4}{ext}";
            //File.WriteAllBytes(Path.Combine(ddir, fname), FileBytes);
            //FileDir = subdir;
            //OldFileName = FileName;
            //FileName = fname;
        }

        public void Load()
        {
            throw new NotImplementedException();
            try
            {
                //FileBytes = File.ReadAllBytes(Path.Combine(SqlContext.FileDirectory, FileDir, FileName));
            }
            catch (Exception)
            {
                // 
            }
        }
    }
}