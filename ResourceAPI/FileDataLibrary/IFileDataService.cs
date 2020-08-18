using System.Collections.Generic;

namespace FileDataLibrary
{
    public interface IFileDataService
    {
        //public FileData Create(FileDataView fileData, int problemId = 0,int exerciseId=0,int quizTestId=0,int quizQuestionId=0,int quizAnswerId=0);
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
    }
}