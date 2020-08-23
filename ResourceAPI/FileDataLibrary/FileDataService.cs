using System;
using System.Collections.Generic;
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

        public FileData CreateForProblem(FileDataView fileData, int problemId)
        {
            return Create(fileData, problemId);
        }

        public FileData CreateForExercise(FileDataView fileData, int exerciseId)
        {
            return Create(fileData, exerciseId: exerciseId);
        }

        public FileData CreateForQuizTest(FileDataView fileData, int quizTestId)
        {
            return Create(fileData, quizTestId: quizTestId);
        }

        public FileData CreateForQuizQuestion(FileDataView fileData, int quizQuestionId)
        {
            return Create(fileData, quizQuestionId: quizQuestionId);
        }

        public FileData CreateForQuizAnswer(FileDataView fileData, int quizAnswerId)
        {
            return Create(fileData, quizAnswerId: quizAnswerId);
        }

        public FileDataView Get(int id)
        {
            var element = _context.FileData.FirstOrDefault(f => f.Id == id);
            if (element == null) return null;
            return ConvertToFileDataView(element);
        }

        public void Delete(int id)
        {
            var element = _context.FileData.FirstOrDefault(f => f.Id == id);
            if (element == null) return;
            DeleteFile(element);
            _context.FileData.Remove(element);
            _context.SaveChanges();
        }

        public string GetAbsolutePath(FileData data)
        {
            var relativePath = Path.Join(data.FileDir, data.FileName);
            var absolutePath = Path.GetFullPath(relativePath);
            return absolutePath;
        }

        public IEnumerable<FileDataView> GetFilesForProblem(int problemId)
        {
            var files = _context
                .FileData
                .Where(f => f.ProblemId == problemId)
                .ToList()
                .Select(ConvertToFileDataView);
            return files;
        }

        public FileData CreateFile(FileDataView element)
        {
            return Create(element);
        }

        public IEnumerable<FileDataView> GetFilesForExercise(int exerciseId)
        {
            var files = _context.FileData.Where(f => f.ExerciseId == exerciseId).ToList().Select(ConvertToFileDataView);
            return files;
        }

        public FileData CreateForProblemAnswer(int answerId, FileDataView file)
        {
            return Create(file, problemAnswerId: answerId);
        }

        public IEnumerable<FileDataView> GetFilesForProblemAnswer(int answerId)
        {
            return _context.FileData.Where(f => f.ProblemAnswerId == answerId).ToList()
                .Select(ConvertToFileDataView);
        }

        public IEnumerable<FileDataView> GetFilesForQuizAnswer(int answerId)
        {
            return _context.FileData.Where(f => f.QuizAnswerId == answerId).ToList().Select(ConvertToFileDataView);
        }

        public IEnumerable<FileDataView> GetFilesForQuizQuestion(int questionId)
        {
            return _context.FileData.Where(f => f.QuizQuestionId == questionId).ToList().Select(ConvertToFileDataView);
        }

        public IEnumerable<FileDataView> GetFilesForQuizTest(int testId)
        {
            return _context.FileData.Where(f => f.QuizTestId == testId).ToList().Select(ConvertToFileDataView);
        }

        public int GetDataBaseFilesCount()
        {
            return _context.FileData.Count();
        }

        public int GetFileSystemFilesCount()
        {
            if (!Directory.Exists(FileDirectory)) return 0;
            return Directory.GetFiles(FileDirectory, "*", SearchOption.AllDirectories).Length;
        }

        public void ClearFileSystem()
        {
            if (Directory.Exists(FileDirectory)) Directory.Delete(FileDirectory, true);
        }

        public void DeleteAllForProblem(int problemId)
        {
            var files = _context.FileData.Where(f => f.ProblemId == problemId).ToList();
            foreach (var file in files) DeleteFile(file);
            _context.FileData.RemoveRange(files);
            _context.SaveChanges();
        }

        public FileData Create(FileDataView fileData, int problemId = 0, int exerciseId = 0, int quizTestId = 0,
            int quizQuestionId = 0, int quizAnswerId = 0, int problemAnswerId = 0)
        {
            var path = CreateFile(fileData.FileBytes);

            var element = new FileData
            {
                OriginalFileName = fileData.FileName,
                FileName = Path.GetFileName(path),
                FileDir = Path.GetDirectoryName(path),
                ProblemId = problemId,
                ExerciseId = exerciseId,
                QuizTestId = quizTestId,
                QuizQuestionId = quizQuestionId,
                QuizAnswerId = quizAnswerId,
                ProblemAnswerId = problemAnswerId
            };

            _context.FileData.Add(element);
            _context.SaveChanges();

            return element;
        }

        private FileDataView ConvertToFileDataView(FileData element)
        {
            return new FileDataView
            {
                FileBytes = GetFile(element),
                FileName = element.OriginalFileName
            };
        }

        private byte[] GetFile(FileData fileData)
        {
            return File.ReadAllBytes(Path.Combine(fileData.FileDir, fileData.FileName));
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
            var fpath = GetAbsolutePath(element);
            if (!File.Exists(fpath)) return;
            File.Delete(fpath);
        }
    }
}