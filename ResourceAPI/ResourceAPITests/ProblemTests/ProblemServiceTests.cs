using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ResourceAPI;
using ResourceAPI.ApiServices;
using ResourceAPI.ApiServices.Interfaces;
using ResourceAPI.Models.Problem;
using Xunit;

namespace ResourceAPITests.ProblemTests
{
    public class ProblemServiceTests
    {
        public ProblemServiceTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString());
            var context = new SqlContext(optionsBuilder.Options);
            _problemService = new ProblemService(context);
            _authorService = new AuthorService(context);
        }

        private readonly IAuthorService _authorService;

        private readonly IProblemService _problemService;

        [Fact]
        public void ApproveAnswersTest()
        {
            // Tworzymy nowy problem.
            var id = _problemService.Create(new Problem {Name = "xxx", Content = "yyy"});

            // Tworzymy nową odpowiedź.
            var answerId1 = _problemService.CreateAnswer(id, new Answer {Content = "xxx"});
            var answerId2 = _problemService.CreateAnswer(id, new Answer {Content = "yyy"});
            var answerId3 = _problemService.CreateAnswer(id, new Answer {Content = "zzz"});

            // Pobieramy problem i sprawdzamy czy dostajemy odpowiedzi.
            var problem = _problemService.Get(id);

            // Sprawdzamy czy treść odpowiedzi się zgadza.
            Assert.Contains("xxx", problem.Answers.First(a => a.Id == answerId1).ContentHtml);
            Assert.Contains("yyy", problem.Answers.First(a => a.Id == answerId2).ContentHtml);
            Assert.Contains("zzz", problem.Answers.First(a => a.Id == answerId3).ContentHtml);

            // Sprawdzamy czy odpowiedzi oznaczone są jako niezatwierdzone.
            Assert.False(problem.Answers.First(a => a.Id == answerId1).IsApproved);
            Assert.False(problem.Answers.First(a => a.Id == answerId2).IsApproved);
            Assert.False(problem.Answers.First(a => a.Id == answerId3).IsApproved);

            // Zatwierdzamy odpowiedź.
            _problemService.SetAnswerApproval(id, answerId1);

            // Pobrany problem powinien figurować jako rozwiązany.
            var problem1 = _problemService.Get(id);
            Assert.True(problem1.IsAnswered);
            Assert.True(problem1.Answers.First(a => a.Id == answerId1).IsApproved);
            Assert.False(problem1.Answers.First(a => a.Id == answerId2).IsApproved);
            Assert.False(problem1.Answers.First(a => a.Id == answerId3).IsApproved);

            // Zatwierdzamy inną odpowiedź.
            _problemService.SetAnswerApproval(id, answerId2);

            // Przy zatwierdzeniu innego rozwiązania odpowiedź pierwsza nie powinna być dalej zatwierdzona.
            var problem2 = _problemService.Get(id);
            Assert.True(problem2.IsAnswered);
            Assert.False(problem2.Answers.First(a => a.Id == answerId1).IsApproved);
            Assert.True(problem2.Answers.First(a => a.Id == answerId2).IsApproved);
            Assert.False(problem2.Answers.First(a => a.Id == answerId3).IsApproved);

            // Zdejmujemy zatwierdzenie odpowiedzi.
            _problemService.SetAnswerApproval(id, answerId2, false);

            // Teraz problem powinien figurować jako nierozwiązany.
            var problem3 = _problemService.Get(id);
            Assert.False(problem3.IsAnswered);
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
            var problemId = _problemService.Create(new Problem {Name = "xxx"});

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
            var id = _problemService.Create(new Problem {Name = "xxx"});
            var problem = _problemService.Get(id);
            Assert.Equal(id, problem.Id);
        }

        [Fact]
        public void DeleteAnswerTest()
        {
            // Tworzymy nowy problem.
            var problemId = _problemService.Create(new Problem {Name = "xxx"});

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
            var id = _problemService.Create(new Problem {Name = "xxx"});

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
            var problemId = _problemService.Create(new Problem {Name = "xxx"});

            // Tworzymy nowe odpowiedzi.
            var answerId = _problemService.CreateAnswer(problemId, new Answer {Content = "aaa"});

            // Pobieramy odpowiedź.
            var answer1 = _problemService.GetAnswer(problemId, answerId);
            Assert.Equal(answerId, answer1.Id);
            Assert.Contains("aaa", answer1.ContentHtml);

            // Edytujemy odpowiedź.
            _problemService.EditAnswer(problemId, answerId, new Answer {Content = "bbb"});

            // Porównujemy nową opdowiedź ze zmienioną zawartością.
            var answer2 = _problemService.GetAnswer(problemId, answerId);
            Assert.Equal(answerId, answer2.Id);
            Assert.Contains("bbb", answer2.ContentHtml);
        }


        [Fact]
        public void EditProblemTest()
        {
            // Tworzymy nowy problem.
            var id = _problemService.Create(new Problem {Name = "xxx"});

            // Wartości powinny być takie jak utworzone.
            var problem = _problemService.Get(id);
            Assert.Equal(id, problem.Id);
            Assert.Equal("xxx", problem.Name);

            // Edytujemy wartości problemu.
            problem.Name = "yyy";
            var res = _problemService.Edit(id, problem);
            Assert.True(res);

            // Nowe wartości powinny być odpowiednie do wprowadzonych.
            var problem2 = _problemService.Get(id);
            Assert.Equal(id, problem2.Id);
            Assert.Equal("yyy", problem2.Name);
        }

        [Fact]
        public void GetProblemTest()
        {
            // Tworzymy nowy problem.
            var id = _problemService.Create(new Problem {Name = "xxx", Content = "yyy"});

            // Wartości powinny być takie jak utworzone.
            var problem = _problemService.Get(id);
            Assert.Equal(id, problem.Id);
            Assert.Equal("xxx", problem.Name);
            Assert.Contains("yyy", problem.ContentHtml);
        }
    }
}