using System;
using System.Linq;
using System.Reflection;
using CommonLibrary;
using FileDataLibrary;
using Microsoft.EntityFrameworkCore;
using ProblemLibrary;
using ResourceAPI;
using Xunit;

namespace ResourceAPITests.ProblemTests
{
    public class ProblemServiceTests
    {
        public ProblemServiceTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var context = new SqlContext(optionsBuilder.Options);

            var problemLibraryAssembly = Assembly.Load("ProblemLibrary");
            _fileDataService = new FileDataService(context);
            _problemService = new ProblemService(context, _fileDataService);
            _authorService = new AuthorService(context);
        }

        private readonly IAuthorService _authorService;
        private readonly IFileDataService _fileDataService;
        private readonly IProblemService _problemService;

        [Fact]
        public void ApproveAnswersTest()
        {
            // Tworzymy nowy problem.
            var id = _problemService.Create(new Problem {Name = "xxx", Content = "yyy"}).Id;

            // Tworzymy nową odpowiedź.
            var answerId1 = _problemService.CreateAnswer(id, new Answer {Content = "xxx"});
            var answerId2 = _problemService.CreateAnswer(id, new Answer {Content = "yyy"});
            var answerId3 = _problemService.CreateAnswer(id, new Answer {Content = "zzz"});

            // Pobieramy problem i sprawdzamy czy dostajemy odpowiedzi.
            var problem = _problemService.Get(id);

            // Sprawdzamy czy treść odpowiedzi się zgadza.
            Assert.Contains("xxx", problem.Answers.First(a => a.Id == answerId1).Content);
            Assert.Contains("yyy", problem.Answers.First(a => a.Id == answerId2).Content);
            Assert.Contains("zzz", problem.Answers.First(a => a.Id == answerId3).Content);

            // Sprawdzamy czy odpowiedzi oznaczone są jako niezatwierdzone.
            Assert.False(problem.Answers.First(a => a.Id == answerId1).IsApproved);
            Assert.False(problem.Answers.First(a => a.Id == answerId2).IsApproved);
            Assert.False(problem.Answers.First(a => a.Id == answerId3).IsApproved);

            // Zatwierdzamy odpowiedź.
            _problemService.SetAnswerApproval(id, answerId1);

            // Pobrany problem powinien figurować jako rozwiązany.
            var problem1 = _problemService.Get(id);
            Assert.True(problem1.IsSolved);
            Assert.True(problem1.Answers.First(a => a.Id == answerId1).IsApproved);
            Assert.False(problem1.Answers.First(a => a.Id == answerId2).IsApproved);
            Assert.False(problem1.Answers.First(a => a.Id == answerId3).IsApproved);

            // Zatwierdzamy inną odpowiedź.
            _problemService.SetAnswerApproval(id, answerId2);

            // Przy zatwierdzeniu innego rozwiązania odpowiedź pierwsza nie powinna być dalej zatwierdzona.
            var problem2 = _problemService.Get(id);
            Assert.True(problem2.IsSolved);
            Assert.False(problem2.Answers.First(a => a.Id == answerId1).IsApproved);
            Assert.True(problem2.Answers.First(a => a.Id == answerId2).IsApproved);
            Assert.False(problem2.Answers.First(a => a.Id == answerId3).IsApproved);

            // Zdejmujemy zatwierdzenie odpowiedzi.
            _problemService.SetAnswerApproval(id, answerId2, false);

            // Teraz problem powinien figurować jako nierozwiązany.
            var problem3 = _problemService.Get(id);
            Assert.False(problem3.IsSolved);
            Assert.False(problem3.Answers.First(a => a.Id == answerId1).IsApproved);
            Assert.False(problem3.Answers.First(a => a.Id == answerId2).IsApproved);
            Assert.False(problem3.Answers.First(a => a.Id == answerId3).IsApproved);
        }

        [Fact]
        public void BrowseProblemsTest()
        {
            var num1 = _problemService.BrowseProblems(0, out _).Count();
            _problemService.Create(new Problem {Name = "xxx"});
            _problemService.Create(new Problem {Name = "xxx"});
            _problemService.Create(new Problem {Name = "xxx"});
            var num2 = _problemService.BrowseProblems(0, out _).Count();
            Assert.Equal(num1 + 3, num2);
        }

        [Fact]
        public void CreateAnswerTest()
        {
            // Tworzymy nowy problem.
            var problemId = _problemService.Create(new Problem {Name = "xxx"}).Id;

            // Tworzymy nowe odpowiedzi.
            var aid1 = _problemService.CreateAnswer(problemId, new Answer {Content = "aaa"});
            var aid2 = _problemService.CreateAnswer(problemId, new Answer {Content = "bbb"});
            var aid3 = _problemService.CreateAnswer(problemId, new Answer {Content = "ccc"});

            // Weryfikujemy czy liczba odpowiedzi do problemu się zgadza.
            var problem = _problemService.Get(problemId);
            Assert.Equal("xxx", problem.Name);
            Assert.Equal(3, problem.Answers.Count);
        }

        [Fact]
        public void CreateProblemTest()
        {
            var id = _problemService.Create(new Problem {Name = "xxx"}).Id;
            var problem = _problemService.Get(id);
            Assert.Equal(id, problem.Id);
        }

        [Fact]
        public void DeleteAnswerTest()
        {
            // Tworzymy nowy problem.
            var problemId = _problemService.Create(new Problem {Name = "xxx"}).Id;

            // Tworzymy nowe odpowiedzi.
            var answer1Id = _problemService.CreateAnswer(problemId, new Answer {Content = "aaa"});
            var answer2Id = _problemService.CreateAnswer(problemId, new Answer {Content = "bbb"});
            var answer3Id = _problemService.CreateAnswer(problemId, new Answer {Content = "ccc"});

            // Sprawdzamy liczbę odpowiedzi do problemu.
            var problem1 = _problemService.Get(problemId);
            Assert.Equal(3, problem1.Answers.Count);

            // Usuwamy odpowiedź.
            _problemService.DeleteAnswer(problemId, answer1Id);

            // Ponownie sprawdzamy liczbę odpowiedzi do problemu. Powinna być inna.
            var problem2 = _problemService.Get(problemId);
            Assert.Equal(2, problem2.Answers.Count);

            // Usuwamy odpowiedź.
            _problemService.DeleteAnswer(problemId, answer2Id);

            // Ponownie sprawdzamy liczbę odpowiedzi do problemu. Powinna być inna.
            var problem3 = _problemService.Get(problemId);
            Assert.Equal(1, problem3.Answers.Count);
        }

        [Fact]
        public void DeleteProblemTest()
        {
            // Tworzymy nowy problem.
            var id = _problemService.Create(new Problem {Name = "xxx"}).Id;

            // Wartości powinny być takie jak utworzone.
            var problem = _problemService.Get(id);
            Assert.Equal(id, problem.Id);
            Assert.Equal("xxx", problem.Name);

            // Usuwamy problem.

            _problemService.Delete(id);
        }

        [Fact]
        public void EditAnswerTest()
        {
            // Tworzymy nowy problem.
            var problemId = _problemService.Create(new Problem {Name = "xxx"}).Id;

            // Tworzymy nowe odpowiedzi.
            var answerId = _problemService.CreateAnswer(problemId, new Answer {Content = "aaa"});

            // Pobieramy odpowiedź.
            var answer1 = _problemService.GetAnswer(problemId, answerId);
            Assert.Equal(answerId, answer1.Id);
            Assert.Contains("aaa", answer1.Content);

            // Edytujemy odpowiedź.
            _problemService.EditAnswer(problemId, answerId, new Answer {Content = "bbb"});

            // Porównujemy nową opdowiedź ze zmienioną zawartością.
            var answer2 = _problemService.GetAnswer(problemId, answerId);
            Assert.Equal(answerId, answer2.Id);
            Assert.Contains("bbb", answer2.Content);
        }

        [Fact]
        public void FileAnswerCreate()
        {
            var problem = new ProblemUserModel {Content = "aaa", Name = "bbb"};
            var problemLink = _problemService.Create(problem.ToModel());

            var element = new AnswerUserModel
            {
                Content = "![](a.png) ![](b.png) ![](c.png)",
                Files = new[]
                {
                    new FileDataView {FileName = "a.png", FileBytes = Convert.FromBase64String("aaaa")},
                    new FileDataView {FileName = "b.png", FileBytes = Convert.FromBase64String("bbbb")},
                    new FileDataView {FileName = "c.png", FileBytes = Convert.FromBase64String("cccc")}
                }
            };

            var answerId = _problemService.CreateAnswer(problemLink.Id, element.ToModel());

            var answer = _problemService.GetAnswer(problemLink.Id, answerId);

            Assert.Contains("aaaa", answer.Content);
            Assert.Contains("bbbb", answer.Content);
            Assert.Contains("cccc", answer.Content);
        }

        [Fact]
        public void FileProblemCreate()
        {
            var element = new ProblemUserModel
            {
                Name = "abc",
                Content = "cde ![](abc.png) ![](cde.png) ![](efg.png)",
                Files = new[]
                {
                    new FileDataView {FileName = "abc.png", FileBytes = Convert.FromBase64String("aaaa")},
                    new FileDataView {FileName = "cde.png", FileBytes = Convert.FromBase64String("bbbb")},
                    new FileDataView {FileName = "efg.png", FileBytes = Convert.FromBase64String("cccc")}
                }
            };

            var problemLink = _problemService.Create(element.ToModel());
            var problem = _problemService.Get(problemLink.Id);

            Assert.Equal(3, problem.Files.Count());

            var b64List = problem.Files.Select(f => Convert.ToBase64String(f.FileBytes)).ToList();

            Assert.Contains("aaaa", b64List);
            Assert.Contains("bbbb", b64List);
            Assert.Contains("cccc", b64List);

            element.Render();

            Assert.Contains("aaaa", element.Content);
            Assert.Contains("bbbb", element.Content);
            Assert.Contains("cccc", element.Content);
        }


        [Fact]
        public void FileProblemEdit()
        {
            var fsNum1 = _fileDataService.GetFileSystemFilesCount();
            var dbNum1 = _fileDataService.GetDataBaseFilesCount();

            // Tworzymy nowy problem.
            var id = _problemService.Create(new Problem
            {
                Name = "xxx", Content = "![](a.png) ![](b.png) ![](c.png)",
                Files = new[]
                {
                    new FileDataView {FileName = "a.png", FileBytes = Convert.FromBase64String("aaaa")},
                    new FileDataView {FileName = "b.png", FileBytes = Convert.FromBase64String("bbbb")},
                    new FileDataView {FileName = "c.png", FileBytes = Convert.FromBase64String("cccc")}
                }
            }).Id;

            var fsNum2 = _fileDataService.GetFileSystemFilesCount();
            var dbNum2 = _fileDataService.GetDataBaseFilesCount();

            Assert.Equal(fsNum1 + 3, fsNum2);
            Assert.Equal(dbNum1 + 3, dbNum2);

            // Wartości powinny być takie jak utworzone.
            var problem = _problemService.Get(id).ToView();

            Assert.Equal(id, problem.Id);
            Assert.Equal("xxx", problem.Name);

            Assert.Contains("aaaa", problem.Content);
            Assert.Contains("bbbb", problem.Content);
            Assert.Contains("cccc", problem.Content);

            // Edytujemy wartości problemu.
            var problem2 = new ProblemUserModel
            {
                Name = "yyy",
                Content = "![](x.png) ![](y.png) ![](z.png)",
                Files = new[]
                {
                    new FileDataView {FileName = "x.png", FileBytes = Convert.FromBase64String("xxxx")},
                    new FileDataView {FileName = "y.png", FileBytes = Convert.FromBase64String("yyyy")},
                    new FileDataView {FileName = "z.png", FileBytes = Convert.FromBase64String("zzzz")}
                }
            };

            var res = _problemService.Edit(problem2.ToModel(), id);
            Assert.True(res);

            // Nowe wartości powinny być odpowiednie do wprowadzonych.
            var problem3 = _problemService.Get(id).ToView();
            Assert.Equal(id, problem3.Id);
            Assert.Equal("yyy", problem3.Name);
            Assert.Contains("xxxx", problem3.Content);
            Assert.Contains("yyyy", problem3.Content);
            Assert.Contains("zzzz", problem3.Content);

            var fsNum3 = _fileDataService.GetFileSystemFilesCount();
            var dbNum3 = _fileDataService.GetDataBaseFilesCount();

            Assert.Equal(fsNum2, fsNum3);
            Assert.Equal(dbNum2, dbNum3);
        }

        [Fact]
        public void GetProblemTest()
        {
            // Tworzymy nowy problem.
            var id = _problemService.Create(new Problem {Name = "xxx", Content = "yyy"}).Id;

            // Wartości powinny być takie jak utworzone.
            var problem = _problemService.Get(id);
            Assert.Equal(id, problem.Id);
            Assert.Equal("xxx", problem.Name);
            Assert.Contains("yyy", problem.Content);
        }
    }
}