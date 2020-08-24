using System.Collections.Generic;

namespace FileDataLibrary
{
    public interface IFileDataService
    {
        public FileDataView Get(int id);
        public void Delete(int id);
        string GetAbsolutePath(FileData data);
        IEnumerable<FileDataView> GetFilesForProblem(int problemId);
        public FileData CreateForProblem(FileDataView fileData, int problemId);
        public FileData CreateForExercise(FileDataView fileData, int exerciseId);
        public FileData CreateForQuizTest(FileDataView fileData, int quizTestId);
        public FileData CreateForQuizQuestion(FileDataView fileData, int quizQuestionId);
        public FileData CreateForQuizAnswer(FileDataView fileData, int quizAnswerId);
        FileData CreateFile(FileDataView element);
        IEnumerable<FileDataView> GetFilesForExercise(int exerciseId);
        FileData CreateForProblemAnswer(int answerId, FileDataView file);
        IEnumerable<FileDataView> GetFilesForProblemAnswer(int answerId);
        IEnumerable<FileDataView> GetFilesForQuizAnswer(int answerId);
        IEnumerable<FileDataView> GetFilesForQuizQuestion(int questionId);
        IEnumerable<FileDataView> GetFilesForQuizTest(int testId);
        int GetDataBaseFilesCount();
        int GetFileSystemFilesCount();
        void ClearFileSystem();
        void DeleteAllForProblem(int problemId);
        void DeleteForExercise(int exerciseId, string fileFileName);
    }
}